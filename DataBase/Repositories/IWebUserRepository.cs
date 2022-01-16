﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.Repositories
{
    /// <summary>
    ///     Репозиторий пользователей сайта
    /// </summary>
    public interface IWebUserRepository
    {
        /// <summary>
        ///     Создать запрос к таблице пользователей сайта
        /// </summary>
        IQueryable<WebUserEntity> CreateQuery();

        /// <summary>
        ///     Получить пользователя по его Id
        /// </summary>
        Task<WebUserEntity> GetByIdAsync(int webUserId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить пользователя по его имени
        /// </summary>
        Task<WebUserEntity> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать пользователя
        /// </summary>
        Task<WebUserEntity> CreateAsync(Action<WebUserEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить пользователя
        /// </summary>
        Task<WebUserEntity> UpdateAsync(int webUserId, Action<WebUserEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Удалить пользователя
        /// </summary>
        void Remove(WebUserEntity entity);
    }
}
