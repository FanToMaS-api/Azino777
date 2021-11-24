using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Services;
using DataBase.Services.Impl;
using Games.Services;
using NLog;
using Server.GameHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;

// TODO: Добавить ставку в игры (BackLog) => подумать как реализовать
// TODO: Добавить проверки на статус пользователя (Banned, Active, Inactive)
namespace Server.Telegram
{
    /// <summary>
    ///     Модель телеграм бота
    /// </summary>
    internal class TelegramBot : IDisposable
    {
        #region Fields

        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramService _telegramService;

        private readonly CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramBot"/>
        public TelegramBot(string token)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Client = new TelegramBotClient(token);
            _telegramService = new TelegramService(Client);
            _logger.Info("Successful connection to bot");
            _telegramService.OnMessageReceived += HandleUpdateAsync;
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
            cancellationToken = _cancellationTokenSource.Token;
            if (message is null)
            {
                _logger.Error(new NullReferenceException(nameof(message)));
                return;
            }

            await using var dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            using var database = new TelegramDbContext(dbContext);

            if ((await database.Users.GetAsync(message.From.Id, cancellationToken)).UserState.UserStateType == UserStateType.Banned)
            {
                await Client.SendTextMessageAsync(message.Chat, DefaultText.BannedAccountText, cancellationToken: cancellationToken);
                return;
            }

            try
            {
                if (!database.Users.CreateQuery().Any(_ => _.TelegramId == message.From.Id))
                {
                    await database.Users.CreateAsync(CreateUserEntity, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            await HandleUserActionAsync(database, text, message, cancellationToken);

            void CreateUserEntity(UserEntity user)
            {
                user.TelegramId = message.From.Id;
                user.Nickname = message.From.Username;
                user.LastName = message.From.LastName;
                user.ChatId = message.Chat.Id;
                user.FirstName = message.From.FirstName;
                user.LastAction = DateTime.Now;
                user.UserState = new UserStateEntity {
                    Balance = 200,
                    UserId = user.TelegramId,
                    UserStateType = UserStateType.Active,
                };
            }
        }

        /// <summary>
        ///     Обрабатывает команду пользователя
        /// </summary>
        private async Task HandleUserActionAsync(ITelegramDbContext database, string text, Message message, CancellationToken cancellationToken)
        {
            switch (text.ToLower())
            {
                case "/start":
                    {
                        await StartFunctionAsync(database, message, cancellationToken);
                        break;
                    }
                case "/command1": // Профиль пользователя
                    {
                        var info = await GetProfileInfoAsync(database, message.From.Id, cancellationToken);
                        await Client.SendTextMessageAsync(message.Chat, info, cancellationToken: cancellationToken);
                        break;
                    }
                case "/command2": // help
                    {
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.HelpText, cancellationToken: cancellationToken);
                        _logger.Info($"Пользователь с id: {message.From.Id} обратился в поддержку");
                        break;
                    }
                case "/command3": // пополнение средств
                    {
                        await AddCoinAsync(database, message, cancellationToken);
                        _logger.Info($"Пользователь с id: {message.From.Id} запросил пополнение средств");
                        break;
                    }
                case "/command4": // снятие средств
                    {
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.WithdrawFunds, cancellationToken: cancellationToken);
                        _logger.Info($"Пользователь с id: {message.From.Id} запросил вывод средств");
                        break;
                    }
                case "/command5": // 21 очко
                    {
                        var newGame = new BlackjackGameHandler(_telegramService);
                        await newGame.StartBlackJackAsync(database, message.From.Id, cancellationToken);
                        break;
                    }
                case "/command6": // рулетка
                    {

                        var newGame = new RouletteGameHandler(_telegramService);
                        await newGame.StartRouletteAsync(database, message.From.Id, cancellationToken);
                        break;
                    }
            }
        }

        /// <summary>
        ///     Функция обработки первого сообщения от пользователя
        /// </summary>
        private async Task StartFunctionAsync(ITelegramDbContext database, Message message, CancellationToken cancellationToken)
        {
            if (database.Users.CreateQuery().Any(_ => _.TelegramId == message.From.Id))
            {
                await Client.SendTextMessageAsync(message.Chat, DefaultText.HelloText, cancellationToken: cancellationToken);
            }
            else
            {
                await Client.SendTextMessageAsync(message.Chat, DefaultText.FirstHelloText, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        ///     Функция пополнения средств пользователя
        /// </summary>
        private async Task AddCoinAsync(ITelegramDbContext database, Message message, CancellationToken cancellationToken)
        {
            var user = await database.Users.GetAsync(message.From.Id, cancellationToken);

            await database.UserStates.UpdateAsync(user.UserState.Id, UpdateState, cancellationToken);

            static void UpdateState(UserStateEntity stateEntity)
            {
                stateEntity.Balance += 50;
            }

            var info = "Ваш баланс успешно пополнен!\n";
            info += await GetProfileInfoAsync(database, message.From.Id, cancellationToken);
            await Client.SendTextMessageAsync(message.Chat, info, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Возвращает информацию о профиле пользователя
        /// </summary>
        private async Task<string> GetProfileInfoAsync(ITelegramDbContext database, long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await database.Users.GetAsync(userId, cancellationToken);
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

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        #endregion
    }
}
