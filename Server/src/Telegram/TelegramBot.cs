using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Services;
using NLog;
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

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramDbContext _dbContext;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramBot"/>
        public TelegramBot(string token, ITelegramDbContext dbContext)
        {
            _dbContext = dbContext;
            Client = new TelegramBotClient(token);
            _logger.Info("Successful connection to bot");

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
                switch (message.Text.ToLower())
                {
                    case "/start":
                        {
                            if (_dbContext.Users.CreateQuery().Any(_ => _.Id == message.From.Id))
                            {
                                await _dbContext.Users.UpdateAsync(message.From.Id, UpdateUserEntity, cancellationToken);
                                await Client.SendTextMessageAsync(message.Chat, DefaultText.HelloText, cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await _dbContext.Users.CreateAsync(CreateUserEntity, cancellationToken);
                                await Client.SendTextMessageAsync(message.Chat, DefaultText.FirstHelloText, cancellationToken: cancellationToken);
                            }

                            break;
                        }
                    case "help":
                        {

                            await Client.SendTextMessageAsync(message.Chat, DefaultText.HelpInfo, cancellationToken: cancellationToken);
                            _logger.Info($"Пользователь с id: {message.From.Id} обратился в поддержку");
                            break;
                        }
                }
            }

            void CreateUserEntity(UserEntity user)
            {
                user.Id = message.From.Id;
                user.Nickname = message.From.Username;
                user.LastName = message.From.LastName;
                user.FirstName = message.From.FirstName;
                user.LastAction = DateTime.Now;
            }

            void UpdateUserEntity(UserEntity user)
            {
                user.Nickname = message.From.Username;
                user.LastName = message.From.LastName;
                user.FirstName = message.From.FirstName;
                user.LastAction = DateTime.Now;
            }
        }

        /// <summary>
        ///     Обработчик ошибок
        /// </summary>
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                _logger.Error(apiRequestException, apiRequestException.Message);
                throw new Exception(apiRequestException.ToString());
            }
        }

        #endregion
    }
}
