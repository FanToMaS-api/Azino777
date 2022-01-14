namespace DataBase.Types
{
    /// <summary>
    ///     Тип состояния пользователя
    /// </summary>
    public enum UserStateType
    {
        /// <summary>
        ///     Активный
        /// </summary>
        Active,

        /// <summary>
        ///     Заблокировал бота
        /// </summary>
        BlockedBot,

        /// <summary>
        ///     Неактивный
        /// </summary>
        Inactive,

        /// <summary>
        ///     Забанен
        /// </summary>
        Banned
    }
}
