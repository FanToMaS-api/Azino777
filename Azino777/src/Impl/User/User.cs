using System;
using Games.Interfaces.User;

namespace Games.Impl.User
{
    /// <summary>
    ///     Пользователь бота
    /// </summary>
    internal class User : IUser
    {
        #region Fields

        #endregion

        #region .ctor

        /// <inheritdoc cref="User"/>
        public User(IUserState state, string id, string nickname, string phoneNumber)
        {
            State = state;
            Nickname = nickname;
            Id = id;
            LastAction = DateTime.Now;
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
        public DateTime LastAction { get; init; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public double GetBalance()
        {
            return State.Balance;
        }

        #endregion

        #region Private methods

        #endregion
    }
}
