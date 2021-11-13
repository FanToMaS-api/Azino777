using DataBase;
using DataBase.Services.Impl;
using Server.Configuration;
using System;
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
            var _dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            var telegramDbContext = new TelegramDbContext(_dbContext);

            return new TelegramBot(token, telegramDbContext);
        }
    }
}
