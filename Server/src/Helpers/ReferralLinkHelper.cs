using System;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Models;
using DataBase.Repositories;
using Games.Services;
using Telegram.Bot.Types;

namespace Server.Helpers
{
    /// <summary>
    ///     Класс для обработки реферальных ссылок
    /// </summary>
    internal class ReferralLinkHelper
    {
        #region Fields

        private readonly ITelegramService _telegramService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="ReferralLinkHelper"/>
        public ReferralLinkHelper(ITelegramService telegramService)
        {
            _telegramService = telegramService;
            _telegramService.OnMessageReceived += OnLinkRecieved;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Обработчик ввода реферальной ссылки
        /// </summary>
        private async Task OnLinkRecieved(object @object, string link, CancellationToken cancellationToken = default)
        {
            var message = (Message)@object;
            using var database = TelegramDbContextFactory.Create();

            if (await IsValidLinkAsync(database, link, cancellationToken))
            {
                await database.Users.UpdateAsync(message.From.Id, UpdateUserEntity, cancellationToken);
            }

            _telegramService.OnMessageReceived -= OnLinkRecieved;

            void UpdateUserEntity(UserEntity user)
            {
                user.ReferralLink = link;
                user.UserState.Balance += 50;
            }
        }

        /// <summary>
        ///     Проверят валидность ссылки
        /// </summary>
        private async static Task<bool> IsValidLinkAsync(ITelegramDbContext database, string link, CancellationToken token)
        {
            return (await database.ReferralLinks.GetAsync(link, token)) is not null;
        }

        #endregion
    }
}
