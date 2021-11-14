namespace Server.Telegram
{
    /// <summary>
    ///     Стандартные тексты для ответов пользователям
    /// </summary>
    internal static class DefaultText
    {
        /// <summary>
        ///     Почта для вопросов и предложений
        /// </summary>
        private readonly static string HelpEmail = "arkadiy.marifin@bk.ru";

        /// <summary>
        ///     Приветственный текст для нового пользователя
        /// </summary>
        public static string FirstHelloText = "Привет! Если ты нажал на старт, значит ты уже на полпути к победам и новым выйгрышам!";

        /// <summary>
        ///     Приветственный текст для старого пользователя
        /// </summary>
        public static string HelloText = "Рад твоему возвращению! Я ждал тебя)\nПроходи, располагайся в нашей уютной атмосфере!)";

        /// <summary>
        ///     Текст для сообщения пользователям контактов поддержки
        /// </summary>
        public static string HelpText = $"По всем вопросам, предложениям и уточнениям пишите на почту: {HelpEmail}";

        /// <summary>
        ///     Сообщение об ошибке на сервере
        /// </summary>
        public static string ServerErrorText =
            "Произошла ошибка на сервере. Мы все видим и уже стараемся ее исправить. Извините за неудобства";
    }
}
