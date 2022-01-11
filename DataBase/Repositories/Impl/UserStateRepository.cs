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
    ///     Репозиторий состояний пользователя
    /// </summary>
    internal class UserStateRepository : IUserStateRepository
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="UserStateRepository"/>
        public UserStateRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<UserStateEntity> CreateQuery() => _dbContext.UsersStates.AsQueryable();

        /// <inheritdoc />
        public async Task<UserStateEntity> GetAsync(long userId, CancellationToken cancellationToken = default)
        {
            var userState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userId)
                .Include(_ => _.User)
                .FirstOrDefaultAsync(cancellationToken);
            if (userState is null)
            {
                Log.Error($"Cannot find state of user with id: {userId}");
            }

            return userState;
        }

        /// <inheritdoc />
        public async Task<UserStateEntity> CreateAsync(Action<UserStateEntity> action, CancellationToken cancellationToken = default)
        {
            var userState = new UserStateEntity();
            action(userState);

            var conflictState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userState.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictState is not null)
            {
                Log.Error("Cannot add user state, because it already exists");
                return userState;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.UsersStates.AddAsync(userState, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return userState;
        }

        /// <inheritdoc />
        public async Task<UserStateEntity> UpdateAsync(long id, Action<UserStateEntity> action, CancellationToken cancellationToken = default)
        {
            var userState = await _dbContext.UsersStates
                .Where(_ => _.Id == id)
                .Include(_ => _.User)
                .FirstOrDefaultAsync(cancellationToken);
            if (userState == null)
            {
                Log.Error($"Cannot find userState with id: {id}");
                return null;
            }

            action(userState);

            var conflictingState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userState.UserId && _.Id != id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingState != null)
            {
                Log.Error("Cannot add user state, because it already exists");
                return userState;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return userState;
        }

        #endregion
    }
}
