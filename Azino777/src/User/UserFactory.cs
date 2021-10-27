using Games.Impl.User;
using Games.Interfaces.User;

namespace Games.User
{
    /// <summary>
    ///     Фабрика пользователей
    /// </summary>
    internal class UserFactory
    {
        #region Public methods

        /// <summary>
        ///     Инициализирует пользователя бота
        /// </summary>
        public static IUser CreateUser(string id, string nickname)
        {
            var userState = new UserState();
            return new Impl.User.User(userState, id, nickname);
        }

        #endregion
    }
}
