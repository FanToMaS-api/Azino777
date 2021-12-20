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
using WebUI.Pages.Users.Modals;

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

        private EditUserStateModal _editUserStateModal;

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
                    .Include(_ => _.UserState)
                    .Include(_ => _.UserReferralLink)
                    .OrderByDescending(x => x.LastAction)
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

        /// <summary>
        ///     Открывает модальное окно показа и изменения состояния пользователя
        /// </summary>
        private async Task ShowEditUserStateModalAsync(UserStateEntity userState)
        {
            await _editUserStateModal.ShowModalAync(userState);
        }

        /// <summary>
        ///     Удаляет пользователя
        /// </summary>
        private async Task DeleteUserAsync(UserEntity entity)
        {
            using var scope = Scope.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            using var transaction = await database.BeginTransactionAsync();
            try
            {
                database.Users.Remove(entity);
                await database.SaveChangesAsync();

                _users.Remove(entity);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                await transaction.CommitAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        #endregion
    }
}
