using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий историй игр в австралийскую рулетку
    /// </summary>
    public interface IRouletteHistoryRepository
    {
        /// <summary>
        ///     Создать запрос к таблице историй игр в австралийскую рулетку
        /// </summary>
        IQueryable<RouletteHistoryEntity> CreateQuery();

        /// <summary>
        ///     Получить игру в австралийскую рулетку по Id пользовтеля
        /// </summary>
        Task<RouletteHistoryEntity> GetAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать запись в истории игр в австралийскую рулетку
        /// </summary>
        Task<RouletteHistoryEntity> CreateAsync(Action<RouletteHistoryEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить запись в истории игр в австралийскую рулетку
        /// </summary>
        Task<RouletteHistoryEntity> UpdateAsync(long userId, Action<RouletteHistoryEntity> action, CancellationToken cancellationToken = default);
    }
}
