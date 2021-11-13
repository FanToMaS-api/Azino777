using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;

namespace DataBase.Services.Impl
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

        private IRouletteHistoryRepository _rouletteHistory;

        private IUserRepository _users;

        private IUserStateRepository _userStates;

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
        public IBlackjackHistoryRepository BlackjackHistory => _blackjackHistory ??= new BlackjackHistoryRepository(_dbContext);

        /// <inheritdoc />
        public IRouletteHistoryRepository RouletteHistory => _rouletteHistory ??= new RouletteHistoryRepository(_dbContext);

        /// <inheritdoc />
        public IUserRepository Users => _users ??= new UserRepository(_dbContext);

        /// <inheritdoc />
        public IUserStateRepository UserStates => _userStates ??= new UserStateRepository(_dbContext);

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
