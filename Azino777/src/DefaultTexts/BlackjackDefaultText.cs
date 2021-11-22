namespace Games.DefaultTexts
{
    /// <summary>
    ///     Стандартные тексты для игры в блэкджек
    /// </summary>
    internal class BlackjackDefaultText
    {
        /// <summary>
        ///     Сообщение о том, что недостаточно денег на счете
        /// </summary>
        public readonly static string EndOfMoneyText = "Недостаточно денег на счете";

        /// <summary>
        ///     Предложение взять еще карту
        /// </summary>
        public readonly static string IsNeededNewCartText = "Хочешь взять еще карту?";

        /// <summary>
        ///     Сообщение о равенстве счетов игрока и дилера
        /// </summary>
        public readonly static string EqualScoreText = "Удивительно, очков у дилера столько же сколько и у тебя! Придется перераздать";
    }
}
