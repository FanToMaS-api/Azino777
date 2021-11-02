using Games.User;
using Games.User.Impl;

namespace Games.UserFactory
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
        public static IUser CreateUser(string id, string nickname, string phoneNumber)
        {
            var userState = new UserState();
            return new User.Impl.User(userState, id, nickname, phoneNumber);
        }

        #endregion
    }
}
