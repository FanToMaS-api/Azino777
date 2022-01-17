using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebUI.Services.Identity.Impl
{
    /// <summary>
    ///     Определяет пользователя, валидирует креды авторизации
    /// </summary>
    public class IdentityManager : IIdentityManager
    {
        #region Fields

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region .ctor

        /// <inheritdoc cref="IdentityManager"/>
        public IdentityManager(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task<WebUserEntity> VerifyUserCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();

            var user = await database.WebUsers.GetByUsernameAsync(username, cancellationToken);
            if (user is null)
            {
                return null;
            }

            return user.VerifyPassword(password) ? user : null;
        }

        /// <inheritdoc />
        public async Task<WebUserEntity> GetUserFromHttpContextAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            var principal = httpContext.User;
            return await GetUserAsync(principal, cancellationToken);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Получить пользователя из HttpContext
        /// </summary>
        private async Task<WebUserEntity> GetUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!principal.Identity?.IsAuthenticated ?? true)
            {
                return null;
            }

            var claim = principal.FindFirst(ClaimTypes.Name);
            if (claim is null)
            {
                return null;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            var user = await database.WebUsers.GetByUsernameAsync(claim.Value, cancellationToken);

            return user;
        }

        #endregion
    }
}
