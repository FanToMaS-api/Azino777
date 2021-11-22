using System;

namespace Games.User
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
        public long Id { get; init; }

        /// <summary>
        ///     ID чата пользователя
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        ///     Отражает дату последнего действия пользователя
        /// </summary>
        public DateTime LastAction { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает баланас пользователя
        /// </summary>
        public double GetBalance();

        /// <summary>
        ///     Изменяет баланс пользователя
        /// </summary>
        public void AddBalance(double value);

        #endregion
    }
}
