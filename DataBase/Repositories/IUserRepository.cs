using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий пользователей сайта
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        ///     Создать запрос к таблице пользователей сайта
        /// </summary>
        IQueryable<IdentityUser> CreateQuery();

        /// <summary>
        ///     Удалить пользователя
        /// </summary>
        void Remove(IdentityUser entity);

        /// <summary>
        ///     Получить пользователя по его id
        /// </summary>
        Task<IdentityUser> GetAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать пользователя
        /// </summary>
        Task<IdentityUser> CreateAsync(Action<IdentityUser> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить пользователя
        /// </summary>
        Task<IdentityUser> UpdateAsync(string id, Action<IdentityUser> action, CancellationToken cancellationToken = default);
    }
}
