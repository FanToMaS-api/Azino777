using System;
using DataBase.Models;

namespace Games.User.Impl
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
        public User(IUserState state, long id, long chatId, string nickname, double balance)
        {
            State = state;
            Nickname = nickname;
            Id = id;
            _lastAction = DateTime.Now;
            ChatId = chatId;
            state.AddBalance(balance);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IUserState State { get; init; }

        /// <inheritdoc />
        public string Nickname { get; init; }

        /// <inheritdoc />
        public long Id { get; init; }

        /// <inheritdoc />
        public long ChatId { get; set; }

        /// <inheritdoc />
        public DateTime LastAction {
            get => _lastAction;
            set {
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
        public void AddBalance(double value)
        {
            State.AddBalance(value);
        }

        #endregion
    }
}
