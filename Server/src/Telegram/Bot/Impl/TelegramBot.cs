using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Repositories;
using Games.Services;
using NLog;
using Server.GameHandlers.Impl;
using Server.Helpers;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

// TODO: добавить сервис который раз в день будет всех пользоватлей переводить в неактивные,
//          если они не писали боту, также добавить лимит по смс в минуту,
//          при превышении лимита отправлять в бан + добавить причины бана
namespace Server.Telegram.Bot.Impl
{
    /// <summary>
    ///     Модель телеграм бота
    /// </summary>
    internal class TelegramBot : ITelegramBot
    {
        #region Fields

        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramBot"/>
        public TelegramBot(IMessageService messageService)
        {
            _messageService = messageService;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task HandleUpdateAsync(Message message, string text, CancellationToken cancellationToken)
        {
            if (message is null)
            {
                Log.Error(new NullReferenceException(nameof(message)));
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                Log.Warn(new NullReferenceException(nameof(text)));
                return;
            }

            using var database = TelegramDbContextFactory.Create();

            try
            {
                var userId = message.From.Id;
                var chatId = message.Chat.Id;
                if (!database.Users.CreateQuery().Any(_ => _.TelegramId == userId))
                {
                    await StartFunctionAsync(database, message, cancellationToken);
                    var newUser = await database.Users.CreateAsync(CreateUserEntity, cancellationToken);

                    await _messageService.SendAsync(DefaultText.InputReferralLinkText, chatId, cancellationToken);
                    var referralLinklHelper = new ReferralLinkHelper(_messageService, userId);

                    return;
                }

                var user = await database.Users.GetAsync(userId, cancellationToken);
                if (user.UserState.UserStateType == UserStateType.Banned)
                {
                    await _messageService.SendAsync(DefaultText.BannedAccountText, chatId, cancellationToken);
                    return;
                }

                await HandleUserActionAsync(database, text, message, cancellationToken);
            }
            catch (ApiRequestException apiEx)
            {
                if (IsUserBlockedBot(apiEx))
                {
                    var user = await database.Users.GetAsync(message.From.Id, cancellationToken);
                    if (user is null)
                    {
                        return;
                    }

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

        #endregion

        #region Private methods

        /// <summary>
        ///     Обрабатывает команду пользователя
        /// </summary>
        private async Task HandleUserActionAsync(ITelegramDbContext database, string text, Message message, CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var chatId = message.Chat.Id;
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
                        await _messageService.SendAsync(info, chatId, cancellationToken);
                        break;
                    }
                case "/command2": // help
                    {
                        await _messageService.SendAsync(DefaultText.HelpText, chatId, cancellationToken);
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
                        await _messageService.SendAsync(DefaultText.WithdrawFundsText, chatId, cancellationToken);
                        Log.Info($"Пользователь с id: {userId} запросил вывод средств");
                        break;
                    }
                case "/command5": // 21 очко
                    {
                        var newGame = new BlackjackGameHandler(_messageService);
                        await _messageService.SendAsync(DefaultText.InputBidText, chatId, cancellationToken);
                        var gameHelper = new GameHelper(_messageService, newGame, userId);
                        break;
                    }
                case "/command6": // рулетка
                    {
                        var newGame = new RouletteGameHandler(_messageService);
                        await _messageService.SendAsync(DefaultText.InputBidText, chatId, cancellationToken);
                        var gameHelper = new GameHelper(_messageService, newGame, userId);
                        break;
                    }
            }

            await database.Users.UpdateAsync(userId, UpdateUserEntity, cancellationToken);
        }

        /// <summary>
        ///     Обновляет дату последнего действия пользователя
        /// </summary>
        private static void UpdateUserEntity(UserEntity user)
        {
            user.LastAction = DateTime.Now;
        }

        /// <summary>
        ///     Функция обработки первого сообщения от пользователя
        /// </summary>
        private async Task StartFunctionAsync(ITelegramDbContext database, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            if (database.Users.CreateQuery().Any(_ => _.TelegramId == message.From.Id))
            {
                await _messageService.SendAsync(DefaultText.HelloText, chatId, cancellationToken);
            }
            else
            {
                await _messageService.SendAsync(DefaultText.FirstHelloText, chatId, cancellationToken);
            }
        }

        /// <summary>
        ///     Функция пополнения средств пользователя
        /// </summary>
        private async Task AddCoinAsync(ITelegramDbContext database, Message message, CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var user = await database.Users.GetAsync(userId, cancellationToken);

            await database.UserStates.UpdateAsync(user.UserState.Id, UpdateState, cancellationToken);

            static void UpdateState(UserStateEntity stateEntity)
            {
                stateEntity.Balance += 50;
                stateEntity.UserStateType = UserStateType.Active;
            }

            var info = "Ваш баланс успешно пополнен!\n";
            info += await GetProfileInfoAsync(database, userId, cancellationToken);
            await _messageService.SendAsync(info, message.Chat.Id, cancellationToken);
        }

        /// <summary>
        ///     Возвращает информацию о профиле пользователя
        /// </summary>
        private async static Task<string> GetProfileInfoAsync(ITelegramDbContext database, long userId, CancellationToken cancellationToken)
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
        private static bool IsUserBlockedBot(ApiRequestException ex) => ex.Message == "Forbidden: bot was blocked by the user";

        #endregion
    }
}
