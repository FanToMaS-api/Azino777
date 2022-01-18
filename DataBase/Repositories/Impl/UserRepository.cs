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
    internal class UserRepository : IUserRepository
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="UserRepository"/>
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<UserEntity> CreateQuery() => _dbContext.Users.AsQueryable();

        /// <inheritdoc />
        public void Remove(UserEntity entity) => _dbContext.Users.Remove(entity);

        /// <inheritdoc />
        public async Task<UserEntity> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .Include(_ => _.UserState)
                .Include(_ => _.UserReferralLink)
                .Where(_ => _.TelegramId == id)
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<UserEntity> CreateAsync(Action<UserEntity> action, CancellationToken cancellationToken = default)
        {
            var user = new UserEntity();
            action(user);

            var conflictingUser = await _dbContext.Users
                .Where(_ => _.TelegramId == user.TelegramId)
                .FirstOrDefaultAsync(cancellationToken);

            if (conflictingUser != null)
            {
                Log.Error("User with this id is already exist");
                return user;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<UserEntity> UpdateAsync(long id, Action<UserEntity> action, CancellationToken cancellationToken = default)
        {
            var user = await GetAsync(id, cancellationToken);
            if (user is null)
            {
                return user;
            }

            action(user);

            var conflictingUser = await _dbContext.Users
                .Where(_ => _.TelegramId == id && _.Id != user.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingUser != null)
            {
                Log.Error("User with this id is already exist");
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
