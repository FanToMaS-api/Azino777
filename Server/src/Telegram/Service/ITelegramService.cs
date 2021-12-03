using System;
using System.Threading.Tasks;

namespace Server.Telegram.Service
{
    /// <summary>
    ///     Базовый интерфейс управления ботом
    /// </summary>
    public interface ITelegramService : IDisposable
    {
        /// <summary>
        ///     Инициализирует бота
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Запускает бота
        /// </summary>
        Task StartAsync();

        /// <summary>
        ///     Останавливает бота
        /// </summary>
        void Stop();

        /// <summary>
        ///     Перезапускает бота
        /// </summary>
        Task RestartAsync();
    }
}
