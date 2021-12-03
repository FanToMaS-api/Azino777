using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Games.Services
{
    /// <summary>
    ///     Базовый интерфейс для вывода текстовых сообщений
    /// </summary>
    public interface IMessageService
    {
        #region Properties

        /// <summary>
        ///     Событие получения сообщения от пользователя
        /// </summary>
        public event Func<object, string, CancellationToken, Task> OnMessageReceived;

        #endregion

        #region Public methods

        /// <summary>
        ///     Выводит сообщение
        /// </summary>
        Task SendAsync(string message, long chatId, CancellationToken token);

        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        Task HandleUpdateAsync(Message message, string text, CancellationToken cancellationToken);

        #endregion
    }
}
