using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Repositories;
using DataBase.Types;
using Games.Games;
using Games.Games.Impl;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Server.Helpers;
using Server.Mappers;

namespace Server.GameHandlers.Impl
{
    /// <summary>
    ///     Обработчик для игры блэкджек
    /// </summary>
    internal class BlackjackGameHandler : IGameHandler
    {
        #region Fields

        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BlackjackGameHandler"/>
        public BlackjackGameHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task StartGameAsync(ITelegramDbContext dbContext, long userId, double bid, CancellationToken cancellationToken)
        {
            UserEntity userEntity;
            try
            {
                userEntity = await dbContext.Users.GetAsync(userId, cancellationToken);

                var activeGame = await dbContext.BlackjackHistory
                   .CreateQuery()
                   .FirstOrDefaultAsync(_ => _.UserId == userEntity.Id && _.GameState == GameStateType.IsOn, cancellationToken);
                if (activeGame is not null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }

            var user = Mapper.Map(userEntity, userEntity.UserState);
            IGame game = new BlackjackGame(user, _messageService);
            game.OnGameUpdated += OnGameUpdatedAsync;
            game.OnGameEnded += OnGameEnded;

            try
            {
                await dbContext.BlackjackHistory.CreateAsync(CreateBlackjackHistoryRecord, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            await game.StartGameAsync(bid, cancellationToken);

            void CreateBlackjackHistoryRecord(BlackjackHistoryEntity record)
            {
                record.Bid = bid;
                record.DialerScope = 0;
                record.UserScope = 0;
                record.User = userEntity;
                record.GameState = GameStateType.IsOn;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик завершения игры
        /// </summary>
        private async Task OnGameEnded(IGame game, EventArgs e, CancellationToken token)
        {
            game.OnGameEnded -= OnGameEnded;
            game.OnGameUpdated -= OnGameUpdatedAsync;

            using var database = TelegramDbContextFactory.Create();
            try
            {
                var userId = game.User.Id;
                var userState = await database.UserStates.GetAsync(userId, token);
                userState = await database.UserStates.UpdateAsync(userState.Id, UpdateUserStateEntity, token);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                var endedGame = await OnGameUpdatedAsync(game, null, token);
                if (endedGame is not null && endedGame.UserScope <= 21 && endedGame.DialerScope < endedGame.UserScope)
                {
                    await SendAwardToReferralAsync(database, endedGame, token);
                }
            }

            void UpdateUserStateEntity(UserStateEntity state)
            {
                state.Balance = game.User.GetBalance();
                state.UserStateType = UserStateType.Active;
            }
        }

        /// <summary>
        ///     Обновляет дату последнего действия пользователя
        /// </summary>
        private static void UpdateUserEntity(UserEntity user)
        {
            user.LastAction = DateTime.Now;
        }

        /// <summary>
        ///     Обрабатывает события при обновлении игры
        /// </summary>
        private async Task<BlackjackHistoryEntity> OnGameUpdatedAsync(IGame sender, EventArgs e, CancellationToken token = default)
        {
            if (sender is BlackjackGame blackjackGame)
            {
                using var database = TelegramDbContextFactory.Create();

                try
                {
                    var user = await database.Users.UpdateAsync(blackjackGame.User.TelegramId, UpdateUserEntity, token);
                    return await database.BlackjackHistory.UpdateAsync(user.Id, UpdateRecord, token);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                void UpdateRecord(BlackjackHistoryEntity record)
                {
                    record.Bid = blackjackGame.Bid;
                    record.DialerScope = blackjackGame.DialerScope;
                    record.UserScope = blackjackGame.UserScope;
                    record.GameState = e is null ? GameStateType.IsOver : GameStateType.IsOn;
                }
            }

            return null;
        }

        /// <summary>
        ///     Отправляет поощрительную награду за победу реферала
        /// </summary>
        private async Task SendAwardToReferralAsync(ITelegramDbContext database, BlackjackHistoryEntity game, CancellationToken token)
        {
            using var transaction = await database.BeginTransactionAsync(token);
            try
            {
                // тот кто выйграл игру
                var user = await database.Users.GetAsync(game.User.TelegramId, token);
                if (user is null || string.IsNullOrEmpty(user.ReferralLink))
                {
                    return;
                }

                // тот, чья реферальная ссылка указана у победителя
                var referralOwner = await database.Users
                        .CreateQuery()
                        .Where(_ => _.UserReferralLink.ReferralLink == user.ReferralLink)
                        .Include(_ => _.UserState)
                        .FirstOrDefaultAsync(token);
                if (referralOwner is null)
                {
                    return;
                }

                if (user.ReferralLink != user.UserReferralLink.ReferralLink)
                {
                    referralOwner.UserState.Balance += Const.ReferralAward;
                    await _messageService.SendAsync(DefaultText.ReferallAwardText, referralOwner.ChatId, token);
                }
                else
                {
                    // если указана собственная реферальная ссылка
                    user.ReferralLink = string.Empty;
                    await _messageService.SendAsync(DefaultText.SelfReferralErrorText, referralOwner.ChatId, token);
                }

                await database.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while sendingaward to referral link owner");
            }
            finally
            {
                await transaction.CommitAsync(token);
            }
        }

        #endregion
    }
}
