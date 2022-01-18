using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DataBase.Repositories;
using DataBase.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Server.Telegram.Service;
using WebUI.Services.Profile;

namespace WebUI.Shared
{
    /// <summary>
    ///     Главная страница сайта
    /// </summary>
    public partial class MainLayout
    {
        #region Injects

        /// <summary>
        ///     Основный сервис
        /// </summary>
        [Inject]
        private ITelegramService TelegramService { get; set; }

        /// <inheritdoc cref="IProfileService" />
        [Inject]
        private IProfileService ProfileService { get; set; }

        /// <inheritdoc cref="IServiceScopeFactory" />
        [Inject]
        private IServiceScopeFactory Scope { get; set; }

        /// <inheritdoc cref="IHttpContextAccessor"/>
        [Inject]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        /// <inheritdoc cref="NavigationManager"/>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        #endregion

        #region Fields

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <inheritdoc />
        async override protected Task OnAfterRenderAsync(bool firstRender)
        {
            using var scope = Scope.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            using var transaction = await database.BeginTransactionAsync();

            try
            {
                var username = HttpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
                var user = await database.WebUsers.GetByUsernameAsync(username);
                if (user is null)
                {
                    return;
                }

                // закрываю все просроченные сессии
                await database.WebUserSessions.CloseExpiredSessionsAsync(user.Id, DateTime.Now);

                var sessionIdStr = HttpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.SerialNumber);
                var sessionId = 0L;
                if (string.IsNullOrEmpty(sessionIdStr) || !long.TryParse(sessionIdStr, out sessionId))
                {
                    return;
                }

                var session = await database.WebUserSessions.GetByIdAsync(sessionId);
                if (session is null || session.Status != WebUserSessionType.Active)
                {
                    NavigationManager.NavigateTo("/logout", true);
                    return;
                }

                await database.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed confirm authorization");
            }
            finally
            {
                await transaction.CommitAsync();
            }
        }

        /// <summary>
        ///     Запускает сервис
        /// </summary>
        private async Task StartAsync()
        {
            await TelegramService.StartAsync();
        }

        /// <summary>
        ///     Останавливает сервис
        /// </summary>
        private void Stop()
        {
            TelegramService.Stop();
        }

        /// <summary>
        ///     Перезапускает сервис
        /// </summary>
        private async Task RestartAsync()
        {
            await TelegramService.RestartAsync();
        }

        #endregion
    }
}
