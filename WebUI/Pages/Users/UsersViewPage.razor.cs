using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace WebUI.Pages.Users
{
    /// <summary>
    ///     Страница пользователей
    /// </summary>
    public partial class UsersViewPage
    {

        #region Injects

        /// <inheritdoc cref="IServiceScopeFactory" />
        [Inject]
        private IServiceScopeFactory Scope { get; set; }

        #endregion

        #region Fields

        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        private List<UserEntity> _users = new();

        #endregion

        #region Methods

        /// <summary>
        ///     Метод перед показом страницы
        /// </summary>
        protected override async Task OnInitializedAsync() => await RefreshAsync();

        /// <summary>
        ///     Получение пользователей из бд
        /// </summary>
        private async Task RefreshAsync()
        {
            try
            {
                using var scope = Scope.CreateScope();
                using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
                _users = database.Users
                    .CreateQuery()
                    .OrderByDescending(x => x.LastAction)
                    .Include(_ => _.UserReferralLink)
                    .ToList();

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        #endregion
    }
}
