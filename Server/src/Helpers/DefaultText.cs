namespace Server.Helpers
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
        public static string HelpText = $"Пользователь, который введет твою реферальную ссылку, получит дополнительные монеты.\n" +
            $"По всем вопросам, предложениям и уточнениям пишите на почту: {HelpEmail}";

        /// <summary>
        ///     Сообщение об ошибке на сервере
        /// </summary>
        public static string ServerErrorText =
            "Произошла ошибка на сервере. Мы все видим и уже стараемся ее исправить. Извините за неудобства";

        /// <summary>
        ///     Сообщение о забаненном аккаунте пользователя
        /// </summary>
        public static string BannedAccountText = $"Ваш аккаунт был забанен. По всем вопросам обращайтесь на почту: {HelpEmail}";

        /// <summary>
        ///     Сообщение о нереализованной функции
        /// </summary>
        public static string NotFunctionText = "Данная функция пока не работает. Извините за неудобства.";

        /// <summary>
        ///     Сообщение о вводе ставки
        /// </summary>
        public static string InputBidText = "Введите вашу ставку";

        /// <summary>
        ///     Предложение ввести реферальныю ссылку
        /// </summary>
        public static string InputReferralLinkText = "Введите реферальную ссылку\nEсли нет - отправьте любой символ";

        /// <summary>
        ///     Сообщение о неверном вводе ставки
        /// </summary>
        public static string ErrorInputBidText = "Ставка не принята, игра отменена";

        /// <summary>
        ///     Сообщение о выводе денежных средств пользователю
        /// </summary>
        public static string WithdrawFundsText = $"Чтобы вывести денежные средства напишите сумму, номер телефона и свой ник в телеграмме на почту: {HelpEmail}\n" +
                                             "После проверки денежные средства будут зачислены на ваш счет, в течение 2х рабочих дней";

        /// <summary>
        ///     Сообщение о зачислении монет за выйгрыш реферала
        /// </summary>
        public static string ReferallAwardText = $"Пользователь, зарегистрированный по вашей реферальной ссылке, выйграл игру! Вы получатете {Const.ReferralAward} монет";
    }
}
