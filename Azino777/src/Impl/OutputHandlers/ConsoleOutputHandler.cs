using System;
using System.Threading.Tasks;

namespace Games.Impl.OutputHandlers
{
    /// <summary>
    ///     Выводит сообщения в консоль
    /// </summary>
    internal class ConsoleOutputHandler : OutputHandlerBase
    {
        /// <inheritdoc />
        public override async Task PrintAsync(string message)
        {
            Console.WriteLine(message);
        }
    }
}
