using System;

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

        /// <summary>
        ///     Номер телефона
        /// </summary>
        public string PhoneNumber { get; init; }

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
        ///     Возвращает сводную информацию о пользователе
        /// </summary>
        public string GetUserInfo();

        #endregion
    }
}
