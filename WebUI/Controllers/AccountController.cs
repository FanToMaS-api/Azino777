using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Repositories;
using DataBase.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using WebUI.Pages.Account;
using WebUI.Services.Identity;

namespace WebUI.Controllers
{
    /// <summary>
    ///     Осуществляет регистрацию новых пользователей
    /// </summary>
    public class AccountController : Controller
    {
        #region Fields

        /// <inheritdoc cref="IServiceScopeFactory" />
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <inheritdoc cref="IIdentityManager" />
        private readonly IIdentityManager _identityManager;

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Время экспирации сессии авторизированного пользователя
        /// </summary>
        private readonly TimeSpan SessionExpiration = TimeSpan.FromDays(1);

        #endregion

        #region .ctor

        /// <inheritdoc cref="AccountController" />
        public AccountController(IServiceScopeFactory serviceScopeFactory, IIdentityManager identityManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _identityManager = identityManager;
        }

        #endregion

        #region Actions

        /// <summary>
        ///     Осуществляет вход пользователя
        /// </summary>
        [HttpGet("/login")]
        public async Task<IActionResult> Login([FromQuery] string data)
        {
            if (data is null)
            {
                return BadRequest("User data is null");
            }

            var cancellationToken = HttpContext.RequestAborted;
            using var scope = _serviceScopeFactory.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            using var transaction = await database.BeginTransactionAsync(cancellationToken);

            var model = AuthorizationModel.Deserialize(data);
            WebUserEntity user = null;
            WebUserSessionEntity session = null;

            if (!(await database.WebUsers.CreateQuery().ToListAsync(cancellationToken)).Any())
            {
                (user, session) = await CreateAdminAsync(database, model, cancellationToken);
            }

            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed clear cookie");
            }

            if (user is null)
            {
                user = await _identityManager.VerifyUserCredentialsAsync(model.Name, model.Password, cancellationToken);
                if (user is null)
                {
                    Logger.Error("Incorrect credentials");
                    return Unauthorized("Incorrect credentials");
                }
            }

            if (session is null)
            {
                var now = DateTime.Now;
                session = CreateNewSession(user.Id, now);

                // Добавляем и сразу сохраняем, чтобы получить идентификатор сессии
                await database.WebUserSessions.AddAsync(session, cancellationToken);
                await database.SaveChangesAsync(cancellationToken);
            }

            var claimsIdentity = GetClaimsIdentity(user, session);
            var authProperties = CreateAuthenticationProperties(session);
            try
            {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                session.Status = WebUserSessionType.Active;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed authorization");
                return BadRequest("Failed authorization");
            }
            finally
            {
                await database.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }

            return LocalRedirect("~/");
        }

        /// <summary>
        ///     Метод выхода из аккаунта
        /// </summary>
        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var cancellationToken = HttpContext.RequestAborted;
                using var scope = _serviceScopeFactory.CreateScope();
                using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
                using var transaction = await database.BeginTransactionAsync(cancellationToken);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var sessionIdStr = HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber);
                if (string.IsNullOrEmpty(sessionIdStr) || !long.TryParse(sessionIdStr, out var sessionId))
                {
                    throw new Exception($"Failed parsing session id '{sessionIdStr}'");
                }

                await database.WebUserSessions.CloseSessionAsync(sessionId, cancellationToken);

                await database.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return LocalRedirect("~/");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed logout");
                return BadRequest("Failed logout");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Регистрирует первого админа, если бд пустая
        /// </summary>
        private async Task<(WebUserEntity, WebUserSessionEntity)> CreateAdminAsync(ITelegramDbContext database, AuthorizationModel model, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var user = CreateNewUser(model, now, WebUserRoleType.Admin);

            // Добавляем и сразу сохраняем, чтобы получить идентификатор пользователя
            await database.WebUsers.AddAsync(user, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            var session = CreateNewSession(user.Id, now);

            // Добавляем и сразу сохраняем, чтобы получить идентификатор сессии
            await database.WebUserSessions.AddAsync(session, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            return (user, session);
        }

        /// <summary>
        ///     Создает нового пользователя сайта
        /// </summary>
        private WebUserEntity CreateNewUser(AuthorizationModel model, DateTime now, WebUserRoleType roleType)
        {
            var user = new WebUserEntity
            {
                Username = model.Name,
                Role = roleType,
                Created = now,
                Updated = now,
            };

            user.SetPassword(model.Password);

            return user;
        }

        /// <summary>
        ///     Создает новую сессию для пользователя сайта
        /// </summary>
        private WebUserSessionEntity CreateNewSession(long userId, DateTime now)
        {
            var expired = now.Add(SessionExpiration);
            return new WebUserSessionEntity
            {
                Expired = expired,
                Status = WebUserSessionType.Undefined,
                UserId = userId,
            };
        }

        /// <summary>
        ///     Создает параметры аутентификации
        /// </summary>
        private AuthenticationProperties CreateAuthenticationProperties(WebUserSessionEntity session)
        {
            return new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = Request.Host.Value,
                ExpiresUtc = session.Expired.Value.ToUniversalTime(),
            };
        }

        /// <summary>
        ///     Получить требования к идентификации
        /// </summary>
        private static ClaimsIdentity GetClaimsIdentity(WebUserEntity user, WebUserSessionEntity session) =>
            new(GetClaims(user, session), CookieAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        ///     Получить массив требований
        /// </summary>
        private static IEnumerable<Claim> GetClaims(WebUserEntity user, WebUserSessionEntity session) => new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.SerialNumber, session.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };

        #endregion
    }
}
