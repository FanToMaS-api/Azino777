using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataBase.Repositories.Impl
{
    /// <summary>
    ///     Репозиторий пользователей бота
    /// </summary>
    internal class BotUserRepository : IBotUserRepository
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BotUserRepository"/>
        public BotUserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<BotUserEntity> CreateQuery() => _dbContext.BotUsers.AsQueryable();

        /// <inheritdoc />
        public void Remove(BotUserEntity entity)
        {
            _dbContext.BotUsers.Remove(entity);
        }

        /// <inheritdoc />
        public async Task<BotUserEntity> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.BotUsers
                .Include(_ => _.UserState)
                .Include(_ => _.UserReferralLink)
                .Where(_ => _.TelegramId == id)
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<BotUserEntity> CreateAsync(Action<BotUserEntity> action, CancellationToken cancellationToken = default)
        {
            var user = new BotUserEntity();
            action(user);

            var conflictingUser = await _dbContext.BotUsers
                .Where(_ => _.TelegramId == user.TelegramId)
                .FirstOrDefaultAsync(cancellationToken);

            if (conflictingUser != null)
            {
                Log.Error("BotUser with this id is already exist");
                return user;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.BotUsers.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<BotUserEntity> UpdateAsync(long id, Action<BotUserEntity> action, CancellationToken cancellationToken = default)
        {
            var user = await GetAsync(id, cancellationToken);
            if (user is null)
            {
                return user;
            }

            action(user);

            var conflictingUser = await _dbContext.BotUsers
                .Where(_ => _.TelegramId == id && _.Id != user.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingUser != null)
            {
                Log.Error("BotUser with this id is already exist");
                return user;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        #endregion
    }
}
