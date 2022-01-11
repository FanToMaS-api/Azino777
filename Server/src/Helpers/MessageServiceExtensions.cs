using Games.Services;

namespace Server.Helpers
{
    /// <summary>
    ///     Расширения для <see cref="IMessageService"/>
    /// </summary>
    internal static class MessageServiceExtensions
    {
        #region Fields

        private static SpamFilter _spamFilter;

        #endregion

        #region Public methods

        /// <summary>
        ///     Добавляет защиту от спама
        /// </summary>
        public static IMessageService AddSpamFilter(this IMessageService messageService)
        {
            _spamFilter = new SpamFilter(messageService);

            return messageService;
        }

        #endregion
    }
}
