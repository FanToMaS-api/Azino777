using System;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Services
{
    /// <summary>
    ///     Базовый класс для вывода текстовых сообщений
    /// </summary>
    public class InOutHandlerBase
    {
        #region Properties

        /// <summary>
        ///     Событие получения сообщения от пользователя
        /// </summary>
        public EventHandler<string> OnMessageReceived { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Выводит сообщение
        /// </summary>
        public async virtual Task PrintAsync(string message, CancellationToken token)
        { }

        /// <summary>
        ///     Получает ввод пользователя
        /// </summary>
        public async virtual Task InputAsync(CancellationToken token)
        { }

        #endregion
    }
}
