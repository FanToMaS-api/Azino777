using Games.User;
using Games.User.Impl;

namespace Games.UserFactory
{
    /// <summary>
    ///     Фабрика пользователей
    /// </summary>
    public class UserFactory
    {
        #region Public methods

        /// <summary>
        ///     Инициализирует пользователя бота
        /// </summary>
        public static IUser CreateUser(long id, long telegramId, long chatId, string nickname, double balance)
        {
            var userState = new UserState();
            return new User.Impl.User(userState, id, telegramId, chatId, nickname, balance);
        }

        #endregion
    }
}
