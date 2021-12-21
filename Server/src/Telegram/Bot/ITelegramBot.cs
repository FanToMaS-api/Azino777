using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Server.Telegram.Bot
{
    /// <summary>
    ///     Интерфейс бота
    /// </summary>
    public interface ITelegramBot
    {
        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        Task HandleUpdateAsync(Message message, string text, CancellationToken cancellationToken);
    }
}
