﻿using DataBase.Entities;
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
        ///     Сопоставляет <see cref="BotUserEntity"/> с <see cref="IUser"/>
        /// </summary>
        public static IUser Map(BotUserEntity user, BotUserStateEntity state)
        {
            // TODO: AutoMapper ?
            return UserFactory.CreateUser(user.Id, user.TelegramId, user.ChatId, user.Nickname, state.Balance);
        }

        #endregion
    }
}
