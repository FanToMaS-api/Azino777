namespace DataBase.Models
{
    /// <summary>
    ///     Отражает последнее состояние игры
    /// </summary>
    public enum GameStateType
    {
        /// <summary>
        ///     Игра еще идет
        /// </summary>
        IsOn,

        /// <summary>
        ///     Игра закончена
        /// </summary>
        IsOver,
    }
}
