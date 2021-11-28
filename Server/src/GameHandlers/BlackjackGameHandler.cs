using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Services;
using DataBase.Services.Impl;
using Games.Games;
using Games.Games.Impl;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Server.Mappers;

namespace Server.GameHandlers
{
    /// <summary>
    ///     Обработчик для игры блэкджек
    /// </summary>
    internal class BlackjackGameHandler
    {
        #region Fields

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramService _telegramService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BlackjackGameHandler"/>
        public BlackjackGameHandler(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Создает новую игру в блэкджек для пользователя
        /// </summary>
        public async Task StartBlackJackAsync(ITelegramDbContext dbContext, long userId, CancellationToken cancellationToken)
        {
            // TODO: Добавить проверку на активную игру, и если игра активна, новую не начинать
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
                _logger.Error(ex);
                return;
            }

            var user = Mapper.Map(userEntity, userEntity.UserState);
            IGame game = new BlackjackGame(user, _telegramService);
            var bid = 50;
            game.OnGameUpdated += OnGameUpdatedAsync;
            game.OnGameEnded += OnGameEnded;

            try
            {
                await dbContext.BlackjackHistory.CreateAsync(CreateBlackjackHistoryRecord, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                _logger.Error(ex);
            }
            finally
            {
                await OnGameUpdatedAsync(game, null, token);
            }

            void UpdateUserStateEntity(UserStateEntity state)
            {
                state.Balance = game.User.GetBalance();
                state.UserStateType = UserStateType.Active;
            }
        }

        /// <summary>
        ///     ОБновляет дату последнего действия пользователя
        /// </summary>
        private static void UpdateUserEntity(UserEntity user)
        {
            user.LastAction = DateTime.Now;
        }

        /// <summary>
        ///     Обрабатывает события при обновлении игры
        /// </summary>
        private async Task OnGameUpdatedAsync(IGame sender, EventArgs e, CancellationToken token = default)
        {
            if (sender is BlackjackGame blackjackGame)
            {
                using var database = TelegramDbContextFactory.Create();

                try
                {
                    var user = await database.Users.UpdateAsync(blackjackGame.User.Id, UpdateUserEntity, token);
                    await database.BlackjackHistory.UpdateAsync(user.Id, UpdateRecord, token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                void UpdateRecord(BlackjackHistoryEntity record)
                {
                    record.Bid = blackjackGame.Bid;
                    record.DialerScope = blackjackGame.DialerScope;
                    record.UserScope = blackjackGame.UserScope;
                    record.GameState = e is null ? GameStateType.IsOver : GameStateType.IsOn;
                }
            }
        }

        #endregion
    }
}
