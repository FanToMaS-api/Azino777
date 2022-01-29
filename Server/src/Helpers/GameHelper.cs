using System;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Repositories;
using Games.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog;
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

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;

        private readonly IGameHandler _gameHandler;

        private readonly long _userId;

        #endregion

        #region .ctor

        /// <inheritdoc cref="GameHelper"/>
        public GameHelper(IMessageService messageService, IGameHandler gameHandler, long userId)
        {
            _messageService = messageService;
            _gameHandler = gameHandler;
            _userId = userId;
            _messageService.OnMessageReceived += OnBidRecieved;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик ввода ставки
        /// </summary>
        private async Task OnBidRecieved(object @object, string text, CancellationToken cancellationToken = default)
        {
            var message = (Message)@object;
            if (_userId != message.From.Id) // если получили сообщение не от того пользователя
            {
                return;
            }

            try
            {
                if (double.TryParse(text, out var bid) && bid >= 10)
                {
                    using var database = TelegramDbContextFactory.Create();
                    await _gameHandler.StartGameAsync(database, message.From.Id, bid, cancellationToken);
                }
                else
                {
                    await _messageService.SendAsync(DefaultText.ErrorInputBidText, message.Chat.Id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to procces input bid from user");
            }
            finally
            {
                _messageService.OnMessageReceived -= OnBidRecieved;
            }
        }

        #endregion
    }
}
