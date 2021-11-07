using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

namespace Games.Services
{
    /// <summary>
    ///     Отвечает за ввод и вывод данных в telegram
    /// </summary>
    internal class TelegramInOutHandler : InOutHandlerBase
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

        /// <summary>
        ///     Id чата для отправки сообщения
        /// </summary>
        public ChatId ChatId { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async override Task PrintAsync(string message, CancellationToken token)
        {
            if (ChatId is null)
            {
                _logger.Error("ChatId shouldn't be null");
                throw new ArgumentNullException(nameof(ChatId), "ChatId shouldn't be null");
            }

            await _client.SendTextMessageAsync(ChatId, message, cancellationToken: token);
            ChatId = null;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message)
            {
                OnMessageReceived?.Invoke(message, message.Text);
            }
        }

        /// <summary>
        ///     Обработчик ошибок
        /// </summary>
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error(exception);
        }

        #endregion
    }
}
