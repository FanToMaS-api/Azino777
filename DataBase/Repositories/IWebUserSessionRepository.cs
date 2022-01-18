using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий сессий пользователей сайта
    /// </summary>
    public interface IWebUserSessionRepository
    {
        /// <summary>
        ///     Создать запрос к таблице сессий
        /// </summary>
        IQueryable<WebUserSessionEntity> CreateQuery();

        /// <summary>
        ///     Добавить сессию
        /// </summary>
        Task AddAsync(WebUserSessionEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Удалить сессию
        /// </summary>
        void Remove(WebUserSessionEntity entity);

        /// <summary>
        ///     Закрывает просрочившиеся сессии
        /// </summary>
        Task CloseExpiredSessionsAsync(long userId, DateTime now, CancellationToken cancellationToken = default);

        /// <summary>
        ///    Закрывает активную сессию пользователя
        /// </summary>
        Task CloseSessionAsync(long sessionId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Возвращает сессию по её идентификатору
        /// </summary>
        Task<WebUserSessionEntity> GetByIdAsync(long sessionId, CancellationToken cancellationToken = default);
    }
}
