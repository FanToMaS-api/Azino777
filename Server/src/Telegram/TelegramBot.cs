using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Services;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Server.GameHandlers.Impl;
using Server.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

// TODO: Добавить проверки на статус пользователя (Banned, Active, Inactive)
// TODO: добавить сервис который раз в день будет всех пользоватлей переводить в неактивные,
//          если они не писали боту, также добавить лимит по смс в минуту,
//          при превышении лимита отправлять в бан + добавить причины бана
namespace Server.Telegram
{
    /// <summary>
    ///     Модель телеграм бота
    /// </summary>
    internal class TelegramBot : IDisposable
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

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
            _telegramService.OnMessageReceived += HandleUpdateAsync;
            Log.Info("Successful connection to bot");

            ApplyMigrations();

            using var database = TelegramDbContextFactory.Create();
            foreach (var user in database.Users.CreateQuery())
            {
                Log.Debug($"{user.FirstName}\n");
            }
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
        ///     Применяет миграции
        /// </summary>
        private void ApplyMigrations()
        {
            Log.Info("Applying database migrations...");

            using var database = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());

            database.Database.Migrate();

            Log.Info("Database migrations successfully applied");
        }

        /// <summary>
        ///     Обработчик событий обновлений чата
        /// </summary>
        private async Task HandleUpdateAsync(object @object, string text, CancellationToken cancellationToken = default)
        {
            var message = (Message)@object;
            cancellationToken = _cancellationTokenSource.Token;
            if (message is null)
            {
                Log.Error(new NullReferenceException(nameof(message)));
                return;
            }

            using var database = TelegramDbContextFactory.Create();

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
                Log.Error(ex);
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
                        Log.Info($"Пользователь с id: {message.From.Id} обратился в поддержку");
                        break;
                    }
                case "/command3": // пополнение средств
                    {
                        await AddCoinAsync(database, message, cancellationToken);
                        Log.Info($"Пользователь с id: {message.From.Id} запросил пополнение средств");
                        break;
                    }
                case "/command4": // снятие средств
                    {
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.WithdrawFunds, cancellationToken: cancellationToken);
                        Log.Info($"Пользователь с id: {message.From.Id} запросил вывод средств");
                        break;
                    }
                case "/command5": // 21 очко
                    {
                        var newGame = new BlackjackGameHandler(_telegramService);
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.InputBid, cancellationToken: cancellationToken);
                        var gameHelper = new GameHelper(_telegramService, newGame);
                        break;
                    }
                case "/command6": // рулетка
                    {
                        var newGame = new RouletteGameHandler(_telegramService);
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.InputBid, cancellationToken: cancellationToken);
                        var gameHelper = new GameHelper(_telegramService, newGame);
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
                Log.Error(ex);
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
