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
    ///     Репозиторий реферальных ссылок
    /// </summary>
    internal class ReferralLinkRepository : IReferralLinkRepository
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly AppDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="ReferralLinkRepository"/>
        public ReferralLinkRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public IQueryable<ReferralLinkEntity> CreateQuery() => _dbContext.ReferralLinks.AsQueryable();

        /// <inheritdoc />
        public async Task<ReferralLinkEntity> GetAsync(string link, CancellationToken cancellationToken = default) =>
             await _dbContext.ReferralLinks
                .Where(_ => _.ReferralLink == link)
                .FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        public async Task<ReferralLinkEntity> CreateAsync(Action<ReferralLinkEntity> action, CancellationToken cancellationToken = default)
        {
            var link = new ReferralLinkEntity();
            action(link);

            var conflictingLink = await _dbContext.ReferralLinks
                .Where(_ => _.UserId == link.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (conflictingLink != null)
            {
                Log.Error($"Referral Link for this user \"{link.UserId}\" is already exist");
                return link;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.ReferralLinks.AddAsync(link, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return link;
        }

        #endregion
    }
}
