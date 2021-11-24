using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Services
{
    /// <summary>
    ///     Репозиторий историй игр в блэкджэк
    /// </summary>
    public interface IBlackjackHistoryRepository
    {
        /// <summary>
        ///     Создать запрос к таблице  историй игр в блэкджэк
        /// </summary>
        IQueryable<BlackjackHistoryEntity> CreateQuery();

        /// <summary>
        ///     Получить игру в блэкджэк по Id пользовтеля
        /// </summary>
        Task<BlackjackHistoryEntity> GetAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать запись в истории игр в блэкджэк
        /// </summary>
        Task<BlackjackHistoryEntity> CreateAsync(Action<BlackjackHistoryEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить запись в истории игр в блэкджэк
        /// </summary>
        Task<BlackjackHistoryEntity> UpdateAsync(long userId, Action<BlackjackHistoryEntity> action, CancellationToken cancellationToken = default);
    }
}
