using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий пользователей бота
    /// </summary>
    public interface IBotUserRepository
    {
        /// <summary>
        ///     Создать запрос к таблице пользователей
        /// </summary>
        IQueryable<BotUserEntity> CreateQuery();

        /// <summary>
        ///     Удалить пользователя
        /// </summary>
        void Remove(BotUserEntity entity);

        /// <summary>
        ///     Получить пользователя по его id
        /// </summary>
        Task<BotUserEntity> GetAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать пользователя
        /// </summary>
        Task<BotUserEntity> CreateAsync(Action<BotUserEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить пользователя
        /// </summary>
        Task<BotUserEntity> UpdateAsync(long id, Action<BotUserEntity> action, CancellationToken cancellationToken = default);
    }
}
