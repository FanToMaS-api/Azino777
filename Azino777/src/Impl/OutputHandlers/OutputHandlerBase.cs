using System.Threading.Tasks;

namespace Games.Impl.OutputHandlers
{
    /// <summary>
    ///     Базовый класс для вывода текстовых сообщений
    /// </summary>
    public class OutputHandlerBase
    {
        #region Public methods

        /// <summary>
        ///     Выводит сообщение
        /// </summary>
        public virtual async Task PrintAsync(string message)
        { }

        #endregion
    }
}
