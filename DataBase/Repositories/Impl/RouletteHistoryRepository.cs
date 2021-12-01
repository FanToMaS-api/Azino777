using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataBase.Repositories.Impl
{
    /// <summary>
    ///     Репозиторий историй игр в австралийскую рулетку
    /// </summary>
    internal class RouletteHistoryRepository : IRouletteHistoryRepository
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RouletteHistoryRepository"/>
        public RouletteHistoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<RouletteHistoryEntity> CreateQuery() => _dbContext.RouletteHistory.AsQueryable();

        /// <inheritdoc />
        public async Task<RouletteHistoryEntity> GetAsync(long userId, CancellationToken cancellationToken = default)
        {
            var blackjack = await _dbContext.RouletteHistory
                .Where(_ => _.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);
            if (blackjack is null)
            {
                Log.Error($"Cannot find roulette's record for user with id: {userId}");
            }

            return blackjack;
        }

        /// <inheritdoc />
        public async Task<RouletteHistoryEntity> CreateAsync(Action<RouletteHistoryEntity> action, CancellationToken cancellationToken = default)
        {
            var historyRecord = new RouletteHistoryEntity();
            action(historyRecord);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.RouletteHistory.AddAsync(historyRecord, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return historyRecord;
        }

        /// <inheritdoc />
        public async Task<RouletteHistoryEntity> UpdateAsync(long userId, Action<RouletteHistoryEntity> action, CancellationToken cancellationToken = default)
        {
            var historyRecord = await _dbContext.RouletteHistory
                .Where(_ => _.UserId == userId && _.GameState == GameStateType.IsOn)
                .FirstOrDefaultAsync(cancellationToken);
            if (historyRecord == null)
            {
                Log.Error($"Cannot find roulette's record for user with id: {userId}");
                return null;
            }

            action(historyRecord);
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return historyRecord;
        }

        #endregion
    }
}
