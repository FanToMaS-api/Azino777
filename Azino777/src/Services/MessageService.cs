using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Games.Services
{
    /// <summary>
    ///     Отвечает за отправку сообщений в telegram
    /// </summary>
    public class MessageService : IMessageService
    {
        #region Fields

        private readonly ITelegramBotClient _client;

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="MessageService"/>
        public MessageService(ITelegramBotClient client)
        {
            _client = client;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public event Func<object, string, CancellationToken, Task> OnMessageReceived;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task SendAsync(string message, long chatId, CancellationToken token)
        {
            await _client.SendTextMessageAsync(chatId, message, cancellationToken: token);
        }

        /// <inheritdoc />
        public async Task HandleUpdateAsync(Message message, string text, CancellationToken cancellationToken)
        {
            if (OnMessageReceived != null)
            {
                await OnMessageReceived?.Invoke(message, message.Text, cancellationToken);
            }
        }

        #endregion
    }
}
