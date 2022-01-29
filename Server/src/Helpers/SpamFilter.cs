using System;
using System.Threading;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using DataBase.Repositories;
using DataBase.Types;
using Games.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Telegram.Bot.Types;

namespace Server.Helpers
{
    /// <summary>
    ///     Фильтр спама для бота
    /// </summary>
    internal class SpamFilter
    {
        #region Fields

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;

        #endregion

        #region .ctor

        /// <inheritdoc cref="SpamFilter"/>
        public SpamFilter(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.OnMessageReceived += OnMessageReceived;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверка на спам
        /// </summary>
        private async Task OnMessageReceived(object @object, string text, CancellationToken cancellationToken = default)
        {
            using var database = TelegramDbContextFactory.Create();
            using var transaction = await database.BeginTransactionAsync(cancellationToken);
            try
            {
                var message = (Message)@object;
                var user = await database.Users.GetAsync(message.From.Id, cancellationToken);
                if (user is not null && SpamCheck(user))
                {
                    user.UserState.WarningNumber++;
                    if (user.UserState.WarningNumber > Const.LimitWarningNumber)
                    {
                        user.UserState.UserStateType = UserStateType.Banned;
                        user.UserState.BanReason = BanReasonType.Spam;
                        await _messageService.SendAsync($"{DefaultText.BannedAccountText}. Причина бана: спам", user.ChatId, cancellationToken);
                    }
                    else
                    {
                        await _messageService.SendAsync(DefaultText.SpamWarningText, user.ChatId, cancellationToken);
                    }

                    await database.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while spam check");
            }
            finally
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }

        /// <summary>
        ///     Проверят на спам
        /// </summary>
        private static bool SpamCheck(UserEntity user)
        {
            return DateTime.Now - user.LastAction < Const.TimeToDefineSpam;
        }

        #endregion
    }
}
