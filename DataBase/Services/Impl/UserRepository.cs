using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Services.Impl
{
    /// <summary>
    ///     Репозиторий пользователей бота
    /// </summary>
    internal class UserRepository : IUserRepository
    {
        #region Fields

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
        public IQueryable<UserEntity> CreateQuery()
        {
            return _dbContext.Users;
        }

        /// <inheritdoc />
        public async Task<UserEntity> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                .Where(_ => _.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                throw new Exception($"Cannot find user with id: {id}");
                // TODO: не забыть обработать
            }

            return user;
        }

        /// <inheritdoc />
        public async Task<UserEntity> CreateAsync(Action<UserEntity> action, CancellationToken cancellationToken = default)
        {
            var user = new UserEntity();
            action(user);

            var conflictingUser = await _dbContext.Users
                .Where(_ => _.Id == user.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (conflictingUser != null)
            {
                throw new Exception("User with this phone number is already exist");
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
            var user = await _dbContext.Users
                .Where(_ => _.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                throw new Exception($"Cannot find user with id: {id}");
            }

            action(user);

            var conflictingUser = await _dbContext.Users
                .Where(_ => _.Id == user.Id && _.Id != id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingUser != null)
            {
                throw new Exception("User with this phone number is already exist");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        #endregion
    }
}
