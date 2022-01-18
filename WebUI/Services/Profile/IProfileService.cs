using DataBase.Types;

namespace WebUI.Services.Profile
{
    /// <summary>
    ///     Сервис пользователя
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        ///     Авторизован ли пользователь
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        ///     Пользователь
        /// </summary>
        string Username { get; }

        /// <summary>
        ///     Роль пользователя
        /// </summary>
        WebUserRoleType? Role { get; }
    }
}
