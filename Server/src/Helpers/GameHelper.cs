using System.Threading;
using System.Threading.Tasks;
using DataBase;
using Games.Services;
using Server.GameHandlers;
using Telegram.Bot.Types;

namespace Server.Helpers
{
    /// <summary>
    ///     Класс для работы со ставкой пользователя и начала игры
    /// </summary>
    internal class GameHelper
    {
        #region Fields

        private readonly ITelegramService _telegramService;

        private readonly IGameHandler _gameHandler;

        #endregion

        #region .ctor

        /// <inheritdoc cref="GameHelper"/>
        public GameHelper(ITelegramService telegramService, IGameHandler gameHandler)
        {
            _telegramService = telegramService;
            _gameHandler = gameHandler;
            _telegramService.OnMessageReceived += OnBidRecieved;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик ввода ставки
        /// </summary>
        private async Task OnBidRecieved(object @object, string text, CancellationToken cancellationToken = default)
        {
            var message = (Message)@object;

            if (double.TryParse(text, out var bid))
            {
                using var database = TelegramDbContextFactory.Create();
                await _gameHandler.StartGameAsync(database, message.From.Id, bid, cancellationToken);
            }
            else
            {
                await _telegramService.PrintAsync(DefaultText.ErrorInputBid, message.Chat.Id, cancellationToken);
            }

            _telegramService.OnMessageReceived -= OnBidRecieved;
        }

        #endregion
    }
}
