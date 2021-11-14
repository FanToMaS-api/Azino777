using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Games.Services
{
    /// <summary>
    ///     Отвечает за ввод и вывод данных в telegram
    /// </summary>
    public class TelegramInOutHandler : IInOutHandler
    {
        #region Fields

        private readonly ITelegramBotClient _client;

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramInOutHandler"/>
        public TelegramInOutHandler(ITelegramBotClient client, CancellationToken token = default)
        {
            _client = client;
            var updateHandler = new DefaultUpdateHandler(
                HandleUpdateAsync,
                HandleErrorAsync,
                new[] {
                    UpdateType.Message
                });

            _client.StartReceiving(updateHandler, token);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public event Func<object, string, CancellationToken, Task> OnMessageReceived;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task PrintAsync(string message, long chatId, CancellationToken token)
        {
            await _client.SendTextMessageAsync(chatId, message, cancellationToken: token);
        }

        /// <inheritdoc />
        public async Task InputAsync(CancellationToken token) { }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message)
            {
                OnMessageReceived?.Invoke(message, message.Text, cancellationToken);
            }
        }

        /// <summary>
        ///     Обработчик ошибок
        /// </summary>
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error(exception, exception.Message);
            throw new Exception(exception.ToString());
        }

        #endregion
    }
}
