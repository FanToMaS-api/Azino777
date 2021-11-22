using System;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Services
{
    /// <summary>
    ///     Базовый класс для вывода текстовых сообщений
    /// </summary>
    public interface ITelegramService
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
        public Task PrintAsync(string message, long chatId, CancellationToken token);

        #endregion
    }
}
