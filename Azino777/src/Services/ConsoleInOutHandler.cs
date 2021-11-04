using System;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Services
{
    /// <summary>
    ///     Отвечает за ввод и вывод данных в консоль
    /// </summary>
    internal class ConsoleInOutHandler : InOutHandlerBase
    {
        /// <inheritdoc />
        public async override Task PrintAsync(string message, CancellationToken token)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc />
        public async override Task InputAsync(CancellationToken token)
        {
            var message = Console.ReadLine();
            OnMessageReceived?.Invoke(this, message);
        }
    }
}
