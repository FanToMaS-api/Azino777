using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Types;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataBase.Repositories.Impl
{
    /// <inheritdoc cref="IWebUserSessionRepository" />
    internal class WebUserSessionRepository : IWebUserSessionRepository
    {
        #region Fields

        private readonly AppDbContext _dbContext;

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="WebUserSessionRepository" />
        public WebUserSessionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Implementation IWebUserSessionRepository

        /// <inheritdoc />
        public IQueryable<WebUserSessionEntity> CreateQuery() => _dbContext.Sessions.AsQueryable();

        /// <inheritdoc />
        public async Task AddAsync(WebUserSessionEntity entity, CancellationToken cancellationToken = default) =>
            await _dbContext.Sessions.AddAsync(entity, cancellationToken);

        /// <inheritdoc />
        public void Remove(WebUserSessionEntity entity) => _dbContext.Sessions.Remove(entity);

        /// <inheritdoc />
        public async Task CloseExpiredSessionsAsync(long userId, DateTime now, CancellationToken cancellationToken = default)
        {
            var activeSessions = await _dbContext.Sessions
                .Where(_ => _.UserId == userId && _.Status == WebUserSessionType.Active && _.Expired.HasValue && _.Expired.Value < now)
                .ToListAsync(cancellationToken);
            foreach (var session in activeSessions)
            {
                session.Status = WebUserSessionType.Closed;
            }
        }

        /// <inheritdoc />
        public async Task CloseSessionAsync(long sessionId, CancellationToken cancellationToken = default)
        {
            var activeSession = await _dbContext.Sessions
                 .Where(_ => _.Id == sessionId && _.Status == WebUserSessionType.Active)
                 .FirstOrDefaultAsync(cancellationToken);
            if (activeSession != null)
            {
                activeSession.Status = WebUserSessionType.Closed;
            }
        }

        /// <inheritdoc />
        public async Task<WebUserSessionEntity> GetByIdAsync(long sessionId, CancellationToken cancellationToken = default) =>
            await CreateQuery().Include(_ => _.User).FirstOrDefaultAsync(_ => _.Id == sessionId, cancellationToken);

        #endregion
    }
}
