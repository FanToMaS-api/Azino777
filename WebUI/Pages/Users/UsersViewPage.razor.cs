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
using WebUI.Helpers;
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

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        #endregion

        #region Fields

        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        private List<UserEntity> _users = new();

        private UserStateViewModal _userStateViewModal;

        private EditUserStateModal _editUserStateModal;

        private int _totalCount;

        private int _activePage;

        #endregion

        #region Query parameters

        /// <summary>
        ///     Параметр для запроса
        /// </summary>
        private const string PageNumberQueryParameter = "page";

        /// <summary>
        ///     Кол-во пользователей на 1ой странице
        /// </summary>
        private const int UsersOnPage = 10;

        #endregion

        #region Methods

        /// <summary>
        ///     Метод перед показом страницы
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            using var scope = Scope.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            _totalCount = database.Users.CreateQuery().Count();

            await RefreshAsync();
        }

        /// <inheritdoc />
        protected override Task OnLocationChangedAsync() => RefreshAsync();

        /// <summary>
        ///     Получение пользователей из бд
        /// </summary>
        private async Task RefreshAsync()
        {
            try
            {
                using var scope = Scope.CreateScope();
                using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();

                _activePage = NavigationManager.TryGetQueryParametr(PageNumberQueryParameter, out var res, 1) ? res : 1;
                _users = database.Users
                    .CreateQuery()
                    .Include(_ => _.UserState)
                    .Include(_ => _.UserReferralLink)
                    .OrderByDescending(x => x.LastAction)
                    .Skip((_activePage - 1) * UsersOnPage)
                    .Take(UsersOnPage)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        ///     Открывает модальное окно показа и изменения состояния пользователя
        /// </summary>
        private void ShowUserStateViewModal(UserStateEntity userState)
        {
            _userStateViewModal.ShowModal(userState);
        }

        /// <summary>
        ///     Открывает модалку изменений
        /// </summary>
        private void Edit(UserStateEntity userState)
        {
            _editUserStateModal.ShowModal(userState);
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
