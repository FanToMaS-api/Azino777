using System.Threading;
using System.Threading.Tasks;

namespace Games.Impl.Services
{
    /// <summary>
    ///     Базовый класс для вывода текстовых сообщений
    /// </summary>
    public class InOutHandlerBase
    {
        #region Public methods

        /// <summary>
        ///     Выводит сообщение
        /// </summary>
        public async virtual Task PrintAsync(string message, CancellationToken token)
        { }

        /// <summary>
        ///     Получает ввод пользователя
        /// </summary>
        public async virtual Task<string> InputAsync(CancellationToken token)
        {
            return "";
        }

        #endregion
    }
}
