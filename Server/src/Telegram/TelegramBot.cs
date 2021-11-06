using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Server.Telegram
{
    /// <summary>
    ///     Модель телеграм бота
    /// </summary>
    internal class TelegramBot
    {
        #region Fields


        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramBot"/>
        public TelegramBot(string token)
        {
            Client = new TelegramBotClient(token);
            var updateHandler = new DefaultUpdateHandler(
                HandleUpdateAsync,
                HandleErrorAsync,
                new[] {
                    UpdateType.Message
                });

            Client.StartReceiving(updateHandler);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Клиент телеграм бота
        /// </summary>
        public TelegramBotClient Client { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
        {
            if (update.Message is { } message)
            {
                if (message.Text == "/start")
                {
                    await Client.SendTextMessageAsync(message.Chat,
                        "Привет! Если ты нажал на старт, значит ты уже на полпути к победам и новым выйгрышам!", cancellationToken: cancellationToken);
                }
            }
        }

        /// <summary>
        ///     Обработчик ошибок
        /// </summary>
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // TODO: Добавить логирование
            if (exception is ApiRequestException apiRequestException)
            {
                throw new Exception(apiRequestException.ToString());
            }
        }

        #endregion
    }
}
