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
        ///     Приветственный текст для старого пользователя
        /// </summary>
        public static string HelpText = $"По всем вопросам, предложениям и уточнениям пишите на почту: {HelpEmail}";

        public static string HelpInfo = $"Спасибо! Ваше письмо успешно доставлено, ожидайте ответа.\nОн будет выслан вам в этом чате в течении 2х рабочих дней.";
    }
}
