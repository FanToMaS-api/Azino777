using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Services
{
    /// <summary>
    ///     Репозиторий пользователей бота
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        ///     Создать запрос к таблице пользователей
        /// </summary>
        IQueryable<UserEntity> CreateQuery();

        /// <summary>
        ///     Получить пользователя по его телефону
        /// </summary>
        Task<UserEntity> GetAsync(string phoneNumber, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать пользователя
        /// </summary>
        Task<UserEntity> CreateAsync(Action<UserEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить пользователя
        /// </summary>
        Task<UserEntity> UpdateAsync(long id, Action<UserEntity> action, CancellationToken cancellationToken = default);
    }
}
