using System;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Repositories;
using Games.Games;
using Games.Games.Impl;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Server.Mappers;

namespace Server.GameHandlers.Impl
{
    /// <summary>
    ///     Обработчик для игры в Австралийскую рулетку
    /// </summary>
    internal class RouletteGameHandler : IGameHandler
    {
        #region Fields

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramService _telegramService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteGameHandler"/>
        public RouletteGameHandler(ITelegramService telegramService)
        {
            _telegramService = telegramService;
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

                var activeGame = await dbContext.RouletteHistory
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
            IGame game = new RouletteGame(user, _telegramService);
            game.OnGameUpdated += OnGameUpdatedAsync;
            game.OnGameEnded += OnGameEnded;

            try
            {
                await dbContext.RouletteHistory.CreateAsync(CreateRouletteHistoryRecord, cancellationToken);
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
            if (sender is RouletteGame rouletteGame)
            {
                using var database = TelegramDbContextFactory.Create();
                try
                {
                    var user = await database.Users.UpdateAsync(rouletteGame.User.TelegramId, UpdateUserEntity, token);
                    await database.RouletteHistory.UpdateAsync(user.Id, UpdateRecord, token);
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
