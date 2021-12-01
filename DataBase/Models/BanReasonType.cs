namespace DataBase.Models
{
    /// <summary>
    ///     Причины бана
    /// </summary>
    public enum BanReasonType
    {
        /// <summary>
        ///     Спам
        /// </summary>
        Spam,

        /// <summary>
        ///     Поведение, нарушающее правила бота
        /// </summary>
        BadBehavior,

        /// <summary>
        ///     Общая причина бана
        /// </summary>
        Common
    }
}
