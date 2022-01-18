using System;
using System.Security.Claims;
using DataBase.Types;
using Microsoft.AspNetCore.Http;

namespace WebUI.Services.Profile.Impl
{
    /// <inheritdoc cref="IProfileService"/>
    public class ProfileService : IProfileService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region .ctor

        /// <inheritdoc cref="ProfileService"/>
        public ProfileService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsAuthenticated => _httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated ?? false;

        /// <inheritdoc />
        public string Username => _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        /// <inheritdoc />
        public WebUserRoleType? Role => GetRole();

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает роль пользователя
        /// </summary>
        public WebUserRoleType? GetRole()
        {
            if (IsAuthenticated)
            {
                var roleStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                if (!string.IsNullOrEmpty(roleStr) && Enum.TryParse<WebUserRoleType>(roleStr, out var role))
                {
                    return role;
                }
            }

            return null;
        }

        #endregion
    }
}
