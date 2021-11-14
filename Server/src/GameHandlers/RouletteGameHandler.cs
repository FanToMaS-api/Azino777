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
    ///     Обработчик для игры в Австралийскую рулетку
    /// </summary>
    internal class RouletteGameHandler
    {
        #region Fields

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramDbContext _dbContext;

        private readonly IInOutHandler _inOutHandler;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteGameHandler"/>
        public RouletteGameHandler(ITelegramDbContext dbContext, IInOutHandler inOutHandler)
        {
            _dbContext = dbContext;
            _inOutHandler = inOutHandler;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Создает новую игру в австралийскую рулетку для пользователя
        /// </summary>
        public async Task StartRouletteAsync(long userId, CancellationToken cancellationToken)
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
            var game = new RouletteGame(user, _inOutHandler);
            var bid = 10;
            game.OnGameUpdated += OnGameUpdatedAsync;
            game.OnGameEnded += OnGameEnded;

            try
            {
                await _dbContext.RouletteHistory.CreateAsync(CreateRouletteHistoryRecord, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            await game.StartGameAsync(bid, cancellationToken);

            void CreateRouletteHistoryRecord(RouletteHistoryEntity record)
            {
                record.Coin = 0;
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return;
            }

            void UpdateUserStateEntity(UserStateEntity state)
            {
                state.Balance = game.User.GetBalance();
                state.UserStateType = UserStateType.Active;
            }

            await OnGameUpdatedAsync(game, null, token);
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
            if (sender is RouletteGame rouletteGame)
            {
                try
                {
                    var user = await _dbContext.Users.UpdateAsync(rouletteGame.User.Id, UpdateUserEntity, token);
                    var record = await _dbContext.RouletteHistory.GetAsync(user.Id, token) ??
                                 throw new NullReferenceException($"RouletteHistory record is empty for user with id: {user.TelegramId}");

                    await _dbContext.RouletteHistory.UpdateAsync(record.Id, UpdateRecord, token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                void UpdateRecord(RouletteHistoryEntity record)
                {
                    record.Coin = rouletteGame.Coin;
                    record.GameState = e is null ? GameStateType.IsOver : GameStateType.IsOn;
                }
            }
        }

        #endregion
    }
}
