using System;
using DataBase.Models;
using Games.Interfaces.User;

namespace Games.Impl.User
{
    /// <summary>
    ///     Пользователь бота
    /// </summary>
    internal class User : IUser
    {
        #region Fields

        private DateTime _lastAction;

        #endregion

        #region .ctor

        /// <inheritdoc cref="User"/>
        public User(IUserState state, string id, string nickname, string phoneNumber)
        {
            State = state;
            Nickname = nickname;
            Id = id;
            _lastAction = DateTime.Now;
            PhoneNumber = phoneNumber;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IUserState State { get; init; }

        /// <inheritdoc />
        public string Nickname { get; init; }

        /// <inheritdoc />
        public string Id { get; init; }

        /// <inheritdoc />
        public string PhoneNumber { get; init; }

        /// <inheritdoc />
        public DateTime LastAction
        {
            get => _lastAction;
            set
            {
                State.SetUserStateType(UserStateType.Inactive);
                _lastAction = value;
            }
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public double GetBalance()
        {
            LastAction = DateTime.Now;
            return State.Balance;
        }

        /// <inheritdoc />
        public string GetUserInfo()
        {
            LastAction = DateTime.Now;
            var info = $"Ваш баланс: {State.Balance}\n" +
                       $"Ваш статус: {State.UserStateType}";

            return info;
        }

        #endregion

        #region Private methods

        #endregion
    }
}
