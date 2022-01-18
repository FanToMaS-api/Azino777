using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataBase.Repositories.Impl
{
    /// <inheritdoc cref="IWebUserRepository" />
    internal class WebUserRepository : IWebUserRepository
    {
        #region Fields

        private readonly AppDbContext _dbContext;

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="WebUserRepository" />
        public WebUserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<WebUserEntity> CreateQuery() => _dbContext.WebUsers.AsQueryable();

        /// <inheritdoc />
        public void Remove(WebUserEntity entity) => _dbContext.WebUsers.Remove(entity);

        /// <inheritdoc />
        public async Task AddAsync(WebUserEntity entity, CancellationToken cancellationToken = default) =>
            await _dbContext.WebUsers.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public async Task<WebUserEntity> GetByIdAsync(long webUserId, CancellationToken cancellationToken = default)
        {
            return await CreateQuery()
                .Where(_ => _.Id == webUserId)
                .Include(_ => _.Sessions)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<WebUserEntity> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await CreateQuery()
                .Where(_ => _.Username == username)
                .Include(_ => _.Sessions)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<WebUserEntity> UpdateAsync(long webUserId, Action<WebUserEntity> action, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(webUserId, cancellationToken);
            if (user is null)
            {
                return null;
            }

            action(user);

            var conflictingUser = await _dbContext.WebUsers
                .Where(_ => _.Username == user.Username && _.Id != user.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (conflictingUser != null)
            {
                Logger.Error("User with this username is already exist");
                return null;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return user;
        }

        #endregion
    }
}
