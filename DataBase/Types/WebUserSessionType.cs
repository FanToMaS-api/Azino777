namespace DataBase.Types
{
    /// <summary>
    ///     Статус сессии пользователя
    /// </summary>
    public enum WebUserSessionType
    {
        /// <summary>
        ///     Неопределённое состояние во время создания сессии
        /// </summary>
        Undefined = 0,

        /// <summary>
        ///     Активная сессия
        /// </summary>
        Active,

        /// <summary>
        ///     Закрытая сессия (пользователь вышел)
        /// </summary>
        Closed,

        /// <summary>
        ///     Сессия просрочилась
        /// </summary>
        Expired
    }
}
