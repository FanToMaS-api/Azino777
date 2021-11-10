using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Services.Impl
{
    /// <summary>
    ///     Репозиторий состояний пользователя
    /// </summary>
    internal class UserStateRepository : IUserStateRepository
    {
        #region Fields

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
        public IQueryable<UserStateEntity> CreateQuery()
        {
            return _dbContext.UsersStates;
        }

        /// <inheritdoc />
        public async Task<UserStateEntity> GetAsync(long userId, CancellationToken cancellationToken = default)
        {
            var userState = await _dbContext.UsersStates
                .Where(_ => _.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);
            if (userState is null)
            {
                throw new NullReferenceException($"Cannot find state of user with id: {userId}");
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
                throw new Exception("Cannot add user state, because it already exists");
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
                .FirstOrDefaultAsync(cancellationToken);
            if (userState == null)
            {
                throw new Exception($"Cannot find userState with id: {id}");
            }

            action(userState);

            var conflictingUser = await _dbContext.UsersStates
                .Where(_ => _.UserId == userState.UserId && _.Id != id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingUser != null)
            {
                throw new Exception($"State for user with id: {userState.UserId} is already exist");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return userState;
        }

        #endregion
    }
}
