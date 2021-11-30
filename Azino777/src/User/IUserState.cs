using DataBase.Models;
using Games.Games;

namespace Games.User
{
    /// <summary>
    ///     Отражает текущее состояние пользователя
    /// </summary>
    public interface IUserState
    {
        #region Properties

        /// <summary>
        ///     Баланс пользователя
        /// </summary>
        double Balance { get; }

        /// <summary>
        ///     Текущее состояние пользователя
        /// </summary>
        UserStateType UserStateType { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Пополняет баланс пользователя
        /// </summary>
        public void AddBalance(double value);

        /// <summary>
        ///     Устанавливает статус пользователя
        /// </summary>
        public void SetUserStateType(UserStateType type);

        #endregion
    }
}
