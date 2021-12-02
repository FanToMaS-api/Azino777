using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Repositories;
using Games.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using Server.GameHandlers.Impl;
using Server.Helpers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
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
        private static void ApplyMigrations()
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

            try
            {
                // TODO: добавить баннер для защиты от спама
                // TODO: добавить баф для пользователя чей реф ссылкой пользуются => по 1 монете за каждую выйгранную рефералом игру
                var userId = message.From.Id;
                if (!database.Users.CreateQuery().Any(_ => _.TelegramId == userId))
                {
                    await StartFunctionAsync(database, message, cancellationToken);
                    var user = await database.Users.CreateAsync(CreateUserEntity, cancellationToken);

                    await Client.SendTextMessageAsync(message.Chat, DefaultText.InputReferralLink, cancellationToken: cancellationToken);
                    var referralLinklHelper = new ReferralLinkHelper(_telegramService, userId);

                    return;
                }

                if ((await database.Users.GetAsync(userId, cancellationToken)).UserState.UserStateType == UserStateType.Banned)
                {
                    await Client.SendTextMessageAsync(message.Chat, DefaultText.BannedAccountText, cancellationToken: cancellationToken);
                    return;
                }

                await HandleUserActionAsync(database, text, message, cancellationToken);
            }
            catch (ApiRequestException apiEx)
            {
                if (IsUserBlockedBot(apiEx))
                {
                    var user = await database.Users.GetAsync(message.From.Id, cancellationToken);
                    await database.UserStates.UpdateAsync(
                        user.UserState.Id,
                        _ => { _.UserStateType = UserStateType.BlockedBot; },
                        cancellationToken);

                    return;
                }

                Log.Error(apiEx);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            void CreateUserEntity(UserEntity user)
            {
                user.TelegramId = message.From.Id;
                user.Nickname = message.From.Username;
                user.LastName = message.From.LastName;
                user.ChatId = message.Chat.Id;
                user.FirstName = message.From.FirstName;
                user.LastAction = DateTime.Now;
                user.UserState = new UserStateEntity
                {
                    Balance = 200,
                    UserId = user.TelegramId,
                    UserStateType = UserStateType.Active,
                };
                user.UserReferralLink = new ReferralLinkEntity
                {
                    UserId = user.TelegramId,
                    ReferralLink = user.TelegramId.ToString()
                };
            }
        }

        /// <summary>
        ///     Обрабатывает команду пользователя
        /// </summary>
        private async Task HandleUserActionAsync(ITelegramDbContext database, string text, Message message, CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            switch (text.ToLower())
            {
                case "/start":
                    {
                        await StartFunctionAsync(database, message, cancellationToken);
                        break;
                    }
                case "/command1": // Профиль пользователя
                    {
                        var info = await GetProfileInfoAsync(database, userId, cancellationToken);
                        await Client.SendTextMessageAsync(message.Chat, info, cancellationToken: cancellationToken);
                        break;
                    }
                case "/command2": // help
                    {
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.HelpText, cancellationToken: cancellationToken);
                        Log.Info($"Пользователь с id: {userId} обратился в поддержку");
                        break;
                    }
                case "/command3": // пополнение средств
                    {
                        await AddCoinAsync(database, message, cancellationToken);
                        Log.Info($"Пользователь с id: {userId} запросил пополнение средств");
                        break;
                    }
                case "/command4": // снятие средств
                    {
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.WithdrawFunds, cancellationToken: cancellationToken);
                        Log.Info($"Пользователь с id: {userId} запросил вывод средств");
                        break;
                    }
                case "/command5": // 21 очко
                    {
                        var newGame = new BlackjackGameHandler(_telegramService);
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.InputBid, cancellationToken: cancellationToken);
                        var gameHelper = new GameHelper(_telegramService, newGame, userId);
                        break;
                    }
                case "/command6": // рулетка
                    {
                        var newGame = new RouletteGameHandler(_telegramService);
                        await Client.SendTextMessageAsync(message.Chat, DefaultText.InputBid, cancellationToken: cancellationToken);
                        var gameHelper = new GameHelper(_telegramService, newGame, userId);
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
                stateEntity.UserStateType = UserStateType.Active;
            }

            var info = "Ваш баланс успешно пополнен!\n";
            info += await GetProfileInfoAsync(database, message.From.Id, cancellationToken);
            await Client.SendTextMessageAsync(message.Chat, info, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Возвращает информацию о профиле пользователя
        /// </summary>
        private static async Task<string> GetProfileInfoAsync(ITelegramDbContext database, long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await database.Users.GetAsync(userId, cancellationToken);
                return $"Ваш баланс: {user.UserState.Balance}\nВаш статус: {user.UserState.UserStateType}" +
                    $"\nВаша реферальная ссылка: {user.UserReferralLink.ReferralLink}";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return DefaultText.ServerErrorText;
        }

        /// <summary>
        ///     Проверяет заблокировал ли пользователь бота
        /// </summary>
        private bool IsUserBlockedBot(ApiRequestException ex) => ex.Message == "Forbidden: bot was blocked by the user";

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
