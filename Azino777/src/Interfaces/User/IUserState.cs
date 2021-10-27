using Games.Interfaces.Game;

namespace Games.Interfaces.User
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
        ///     Текущая игра
        /// </summary>
        IGame Game { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Пополняет баланс пользователя
        /// </summary>
        public void AddBalance(double value);

        /// <summary>
        ///     Устанавливает игру для пользователя
        /// </summary>
        public void SetGame(IGame game);

        #endregion
    }
}
