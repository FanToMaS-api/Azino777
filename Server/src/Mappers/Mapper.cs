using DataBase.Entities;
using Games.User;
using Games.UserFactory;

namespace Server.Mappers
{
    /// <summary>
    ///     Сопоставляет сущности
    /// </summary>
    internal static class Mapper
    {
        #region Public methods

        /// <summary>
        ///     Сопоставляет <see cref="UserEntity"/> с <see cref="IUser"/>
        /// </summary>
        public static IUser Map(UserEntity user, UserStateEntity state)
        {
            return UserFactory.CreateUser(user.Id, user.TelegramId, user.ChatId, user.Nickname, state.Balance);
        }

        #endregion
    }
}
