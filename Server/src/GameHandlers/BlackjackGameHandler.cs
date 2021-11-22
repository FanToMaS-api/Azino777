using System;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Services;
using Games.Games;
using Games.Games.Impl;
using Games.Services;
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

        private readonly ITelegramDbContext _dbContext;

        private readonly ITelegramService _telegramService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BlackjackGameHandler"/>
        public BlackjackGameHandler(ITelegramDbContext dbContext, ITelegramService telegramService)
        {
            _dbContext = dbContext;
            _telegramService = telegramService;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Создает новую игру в блэкджек для пользователя
        /// </summary>
        public async Task StartBlackJackAsync(long userId, CancellationToken cancellationToken)
        {
            UserEntity userEntity;
            try
            {
                userEntity = await _dbContext.Users.GetAsync(userId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return;
            }

            var user = Mapper.Map(userEntity, userEntity.UserState);
            var game = new BlackjackGame(user, _telegramService);
            var bid = 10;
            game.OnGameUpdated += OnGameUpdatedAsync;
            game.OnGameEnded += OnGameEnded;

            try
            {
                await _dbContext.BlackjackHistory.CreateAsync(CreateBlackjackHistoryRecord, cancellationToken);
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

            try
            {
                var userId = game.User.Id;
                var user = await _dbContext.Users.UpdateAsync(userId, UpdateUserEntity, token);
                await _dbContext.UserStates.UpdateAsync(user.UserState.Id, UpdateUserStateEntity, token);
                await OnGameUpdatedAsync(game, null, token);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                try
                {
                    var user = await _dbContext.Users.UpdateAsync(blackjackGame.User.Id, UpdateUserEntity, token);
                    var record = await _dbContext.BlackjackHistory.GetAsync(user.Id, token) ??
                                 throw new NullReferenceException($"BlackjackHistory record is empty for user with id: {user.TelegramId}");

                    await _dbContext.BlackjackHistory.UpdateAsync(record.Id, UpdateRecord, token);
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
