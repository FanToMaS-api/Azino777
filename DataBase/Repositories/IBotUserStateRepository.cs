using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий состояний пользователя
    /// </summary>
    public interface IBotUserStateRepository
    {
        /// <summary>
        ///     Создать запрос к таблице пользователей
        /// </summary>
        IQueryable<BotUserStateEntity> CreateQuery();

        /// <summary>
        ///     Получить состояние пользователя по его Id
        /// </summary>
        Task<BotUserStateEntity> GetAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать состояние пользователя
        /// </summary>
        Task<BotUserStateEntity> CreateAsync(Action<BotUserStateEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить состояние пользователя
        /// </summary>
        Task<BotUserStateEntity> UpdateAsync(long id, Action<BotUserStateEntity> action, CancellationToken cancellationToken = default);
    }
}
