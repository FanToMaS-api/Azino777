﻿using DataBase.Models;
using Games.Interfaces.Game;
using Games.Interfaces.User;

namespace Games.Impl.User
{
    /// <summary>
    ///     Показывает текущее состояние пользователя
    /// </summary>
    internal class UserState : IUserState
    {
        #region Properties

        /// <inheritdoc />
        public double Balance { get; private set; }

        // TODO: РЕФАКТОРИНГ
        /// <inheritdoc />
        public IGame Game { get; private set; }

        /// <inheritdoc />
        public UserStateType UserStateType { get; private set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void AddBalance(double value)
        {
            Balance += value;
        }

        /// <inheritdoc />
        public void SetGame(IGame game)
        {
            Game = game;
        }

        /// <inheritdoc />
        public void SetUserStateType(UserStateType type)
        {
            UserStateType = type;
        }

        #endregion
    }
}
