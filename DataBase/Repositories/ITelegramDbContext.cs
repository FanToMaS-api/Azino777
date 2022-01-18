using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Контекст базы данных
    /// </summary>
    public interface ITelegramDbContext : IDisposable
    {
        #region Properties

        /// <inheritdoc cref="IBlackjackHistoryRepository"/>
        public IBlackjackHistoryRepository BlackjackHistory { get; }

        /// <inheritdoc cref="IReferralLinkRepository"/>
        public IReferralLinkRepository ReferralLinks { get; }

        /// <inheritdoc cref="IRouletteHistoryRepository"/>
        public IRouletteHistoryRepository RouletteHistory { get; }

        /// <inheritdoc cref="IUserRepository"/>
        public IUserRepository Users { get; }

        /// <inheritdoc cref="IUserStateRepository"/>
        public IUserStateRepository UserStates { get; }

        /// <inheritdoc cref="IWebUserRepository"/>
        public IWebUserRepository WebUsers { get; }

        /// <inheritdoc cref="IWebUserSessionRepository"/>
        public IWebUserSessionRepository WebUserSessions { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Начать транзакцию
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Сохранить контекст БД
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
