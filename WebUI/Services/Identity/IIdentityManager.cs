using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace WebUI.Services.Identity
{
    /// <summary>
    ///     Определяет пользователя, валидирует креды авторизации
    /// </summary>
    public interface IIdentityManager
    {
        /// <summary>
        ///     Проверяет логин и пароль
        /// </summary>
        Task<WebUserEntity> VerifyUserCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получает пользователя из HttpContext
        /// </summary>
        [ItemNotNull]
        Task<WebUserEntity> GetUserFromHttpContextAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
    }
}
