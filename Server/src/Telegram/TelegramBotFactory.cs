using Server.Configuration;
using System.Configuration;

namespace Server.Telegram
{
    /// <summary>
    ///     Создает телеграм бота
    /// </summary>
    internal static class TelegramBotFactory
    {
        /// <summary>
        ///     Создает новую модель телеграмм бота
        /// </summary>
        public static TelegramBot Create()
        {
            var token = ConfigurationManager.AppSettings.Get(Config.TELEGRAM_TOKEN);
            return new TelegramBot(token);
        }
    }
}
