using System;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Impl.Services
{
    /// <summary>
    ///     Выводит сообщения в консоль
    /// </summary>
    internal class ConsoleInOutHandler : InOutHandlerBase
    {
        /// <inheritdoc />
        public async override Task PrintAsync(string message, CancellationToken token)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc />
        public async override Task<string> InputAsync(CancellationToken token)
        {
            return Console.ReadLine();
        }
    }
}
