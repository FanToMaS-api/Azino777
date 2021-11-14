using System;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Services
{
    /// <summary>
    ///     Отвечает за ввод и вывод данных в консоль
    /// </summary>
    internal class ConsoleInOutHandler : IInOutHandler
    {
        /// <inheritdoc />
        public event Func<object, string, CancellationToken, Task> OnMessageReceived;

        /// <inheritdoc />
        public async Task PrintAsync(string message, long chatId, CancellationToken token)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc />
        public async Task InputAsync(CancellationToken token)
        {
            var message = Console.ReadLine();
            OnMessageReceived?.Invoke(this, message, token);
        }
    }
}
