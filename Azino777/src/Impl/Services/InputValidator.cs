using System.Linq;

namespace Games.Impl.Services
{
    /// <summary>
    ///     Проверят ввод пользователя
    /// </summary>
    public abstract class InputValidator
    {
        #region Public methods

        /// <summary>
        ///     Проверяет согласие пользователя
        /// </summary>
        public bool CheckInput(string input)
        {
            var message = input.ToLower();
            var length = input.Length;
            var firstLetter = message.First();
            return firstLetter == 'д' && length == 1 || firstLetter == 'y' && length == 1
                                                     || message == "yes" || message == "да" || message == "ye";
        }

        #endregion
    }
}
