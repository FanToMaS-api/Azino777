using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Services;
using Games.Services;
using NLog;
using Server.GameHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Server.Telegram
{
    /// <summary>
    ///     Модель телеграм бота
    /// </summary>
    internal class TelegramBot : IDisposable
    {
        #region Fields

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramDbContext _dbContext;

        private readonly IInOutHandler _inOutHandler;

        private readonly CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramBot"/>
        public TelegramBot(string token, ITelegramDbContext dbContext)
        {
            _dbContext = dbContext;
            _cancellationTokenSource = new CancellationTokenSource();
            Client = new TelegramBotClient(token);
            _inOutHandler = new TelegramInOutHandler(Client);
            _logger.Info("Successful connection to bot");
            _inOutHandler.OnMessageReceived += HandleUpdateAsync;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Клиент телеграм бота
        /// </summary>
        public TelegramBotClient Client { get; }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        private async Task HandleUpdateAsync(object @object, string text, CancellationToken cancellationToken = default)
        {
            var message = (Message)@object;
            if (message is null)
            {
                _logger.Error(new NullReferenceException(nameof(message)));
                return;
            }

            try
            {
                if (!_dbContext.Users.CreateQuery().Any(_ => _.TelegramId == message.From.Id))
                {
                    await _dbContext.Users.CreateAsync(CreateUserEntity, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            switch (text.ToLower())
            {
                case "/start":
                    {
                        await StartFunctionAsync(message, cancellationToken);
                        break;
                    }
                case "/command1": // Профиль пользователя
                    {
                        var info = await GetProfileInfoAsync(message.From.Id, cancellationToken);
                        await Client.SendTextMessageAsync(message.Chat, info, cancellationToken: cancellationToken);
                        break;
                    }
                case "/command2": // help
                    {
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.HelpText, cancellationToken: cancellationToken);
                        _logger.Info($"Пользователь с id: {message.From.Id} обратился в поддержку");
                        break;
                    }
                case "/command3": // 21 очко
                    {
                        var newGame = new BlackjackGameHandler(_dbContext, _inOutHandler);
                        await newGame.StartBlackJackAsync(message.From.Id, cancellationToken);
                        break;
                    }
                case "/command4": // рулетка
                    {

                        var newGame = new RouletteGameHandler(_dbContext, _inOutHandler);
                        await newGame.StartRouletteAsync(message.From.Id, cancellationToken);
                        break;
                    }
            }

            void CreateUserEntity(UserEntity user)
            {
                user.TelegramId = message.From.Id;
                user.Nickname = message.From.Username;
                user.LastName = message.From.LastName;
                user.ChatId = message.Chat.Id;
                user.FirstName = message.From.FirstName;
                user.LastAction = DateTime.Now;
                user.UserState = new UserStateEntity {
                    Balance = 50,
                    UserId = user.TelegramId,
                    UserStateType = UserStateType.Active,
                };
            }
        }

        /// <summary>
        ///     Функция обработки первого сообщения от пользователя
        /// </summary>
        private async Task StartFunctionAsync(Message message, CancellationToken cancellationToken)
        {
            if (_dbContext.Users.CreateQuery().Any(_ => _.TelegramId == message.From.Id))
            {
                await Client.SendTextMessageAsync(message.Chat, DefaultText.HelloText, cancellationToken: cancellationToken);
            }
            else
            {
                await Client.SendTextMessageAsync(message.Chat, DefaultText.FirstHelloText, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Возвращает информацию о профиле пользователя
        /// </summary>
        private async Task<string> GetProfileInfoAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _dbContext.Users.GetAsync(userId, cancellationToken);
                return $"Ваш баланс: {user.UserState.Balance}\nВаш статус: {user.UserState.UserStateType}";
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return DefaultText.ServerErrorText;
        }

        #endregion

        #region Implementation IDisposable

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        #endregion
    }
}
