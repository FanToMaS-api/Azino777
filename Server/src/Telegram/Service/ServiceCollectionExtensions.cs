using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Server.Telegram.Impl;

namespace Server.Telegram.Service
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        #region Fields

        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        private const string TelegramToken = "TELEGRAM_TOKEN";

        #endregion

        #region Public methods

        /// <summary>
        ///     Регистрация TelegramService
        /// </summary>
        public static void AddTelegramService(this IServiceCollection services, IConfiguration configuration)
        {
            var token = configuration.GetSection(TelegramToken).Value;

            services.AddSingleton<ITelegramService>(
                _ => new TelegramService(token));
        }

        /// <summary>
        ///     Инициализация телеграмм сервиса
        /// </summary>
        public static void InitializeTelegramService(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<ITelegramService>();
            service.Initialize();

            Logger.Debug("Telegram service has been initialized");
        }

        #endregion
    }
}
