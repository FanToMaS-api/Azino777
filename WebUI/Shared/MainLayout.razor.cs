using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Server.Telegram.Service;

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

        #endregion

        #region Methods

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
