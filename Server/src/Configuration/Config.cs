// ReSharper disable InconsistentNaming
namespace Server.Configuration
{
    /// <summary>
    ///     Класс конфигурации приложения
    /// </summary>
    public static class Config
    {
        /// <summary>
        ///     Токен для подключения к боту телеграмма
        /// </summary>
        public const string TELEGRAM_TOKEN = nameof(TELEGRAM_TOKEN);

        /// <summary>
        ///     Строка подключения к базе данных
        /// </summary>
        public const string DATABASE_CONNECTION_STRING = nameof(DATABASE_CONNECTION_STRING);
    }
}
