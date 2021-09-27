namespace Games.Interfaces.User
{
    /// <summary>
    ///     Базовый интерфейс для пользователей
    /// </summary>
    public interface IUser
    {
        #region Properties

        /// <inheritdoc cref="IUserState"/>
        IUserState State { get; }

        /// <summary>
        ///     Никнейм пользователя
        /// </summary>
        public string Nickname { get; init; }

        /// <summary>
        ///     ID пользователя в телеге
        /// </summary>
        public string Id { get; init; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает баланас пользователя
        /// </summary>
        public double GetBalance();

        #endregion
    }
}
