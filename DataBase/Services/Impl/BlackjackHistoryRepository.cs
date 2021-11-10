using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Services.Impl
{
    /// <summary>
    ///     Репозиторий историй игр в блэкджэк
    /// </summary>
    internal class BlackjackHistoryRepository : IBlackjackHistoryRepository
    {
        #region Fields

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BlackjackHistoryRepository"/>
        public BlackjackHistoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<BlackjackHistoryEntity> CreateQuery()
        {
            return _dbContext.BlackjackHistory;
        }

        /// <inheritdoc />
        public async Task<BlackjackHistoryEntity> GetAsync(long userId, CancellationToken cancellationToken = default)
        {
            var blackjack = await _dbContext.BlackjackHistory
                .Where(_ => _.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);
            if (blackjack is null)
            {
                throw new NullReferenceException($"Cannot find blackjack's record for user with id: {userId}");
            }

            return blackjack;
        }

        /// <inheritdoc />
        public async Task<BlackjackHistoryEntity> CreateAsync(Action<BlackjackHistoryEntity> action, CancellationToken cancellationToken = default)
        {
            var blackjackHistoryRecord = new BlackjackHistoryEntity();
            action(blackjackHistoryRecord);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.BlackjackHistory.AddAsync(blackjackHistoryRecord, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return blackjackHistoryRecord;
        }

        /// <inheritdoc />
        public async Task<BlackjackHistoryEntity> UpdateAsync(long id, Action<BlackjackHistoryEntity> action, CancellationToken cancellationToken = default)
        {
            var blackjackHistoryRecord = await _dbContext.BlackjackHistory
                .Where(_ => _.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (blackjackHistoryRecord == null)
            {
                throw new Exception($"Cannot find blackjack record with id: {id}");
            }

            action(blackjackHistoryRecord);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return blackjackHistoryRecord;
        }

        #endregion
    }
}
