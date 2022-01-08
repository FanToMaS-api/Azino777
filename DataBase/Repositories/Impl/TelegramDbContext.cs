using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;

namespace DataBase.Repositories.Impl
{
    /// <summary>
    ///     Контекст базы данных
    /// </summary>
    public sealed class TelegramDbContext : ITelegramDbContext
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetLogger(nameof(TelegramDbContext));

        private readonly AppDbContext _dbContext;

        private IBlackjackHistoryRepository _blackjackHistory;

        private IReferralLinkRepository _referralLinks;

        private IRouletteHistoryRepository _rouletteHistory;

        private IBotUserRepository _botUsers;

        private IBotUserStateRepository _botUserStates;

        private IUserRepository _users;

        private bool _disposed;

        #endregion

        #region .ctor

        /// <summary>
        /// .ctor
        /// </summary>
        public TelegramDbContext(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IReferralLinkRepository ReferralLinks => _referralLinks ??= new ReferralLinkRepository(_dbContext);

        /// <inheritdoc />
        public IBlackjackHistoryRepository BlackjackHistory => _blackjackHistory ??= new BlackjackHistoryRepository(_dbContext);

        /// <inheritdoc />
        public IRouletteHistoryRepository RouletteHistory => _rouletteHistory ??= new RouletteHistoryRepository(_dbContext);

        /// <inheritdoc />
        public IBotUserRepository BotUsers => _botUsers ??= new BotUserRepository(_dbContext);

        /// <inheritdoc />
        public IBotUserStateRepository BotUserStates => _botUserStates ??= new BotUserStateRepository(_dbContext);
        
        /// <inheritdoc />
        public IUserRepository Users => _users ??= new UserRepository(_dbContext);

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
