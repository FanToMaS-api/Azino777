using System;
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
using WebUI.Services.Profile;

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

        /// <inheritdoc cref="IProfileService" />
        [Inject]
        private IProfileService ProfileService { get; set; }

        #endregion

        #region Fields

        private readonly static ILogger Logger = LogManager.GetCurrentClassLogger();

        private UserEntity[] _users = Array.Empty<UserEntity>();

        private UserStateViewModal _userStateViewModal;

        private EditUserStateModal _editUserStateModal;

        private int _totalCount; // общее кол-во записей в запросе

        private int _activePage; // номер активной страницы

        #endregion

        #region Query parameters

        /// <summary>
        ///     Параметр для запроса
        /// </summary>
        private const string PageNumberQueryParameter = "page";

        /// <summary>
        ///     Параметр для фильтрации по имени пользователя
        /// </summary>
        private const string UserNameQueryParameter = "name";

        /// <summary>
        ///     Кол-во пользователей на 1ой странице
        /// </summary>
        private const int ItemsOnPage = 10;

        #endregion

        #region Methods

        /// <summary>
        ///     Метод перед показом страницы
        /// </summary>
        async override protected Task OnInitializedAsync() => await RefreshAsync();

        /// <inheritdoc />
        async override protected Task OnLocationChangedAsync() => await RefreshAsync();

        /// <summary>
        ///     Получение пользователей из бд
        /// </summary>
        private async Task RefreshAsync()
        {
            try
            {
                using var scope = Scope.CreateScope();
                using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();

                _activePage = NavigationManager.TryGetQueryParameter(PageNumberQueryParameter, out var res, 1) ? res : 1;
                _totalCount = database.Users.CreateQuery().Count();

                var queryable = database.Users.CreateQuery();
                if (NavigationManager.TryGetQueryParameter<string>(UserNameQueryParameter, out var filterName))
                {
                    queryable = queryable.Where(_ => _.FirstName.Contains(filterName));
                }

                _totalCount = queryable.Count();

                _users = await queryable
                    .Include(_ => _.UserState)
                    .Include(_ => _.UserReferralLink)
                    .OrderByDescending(x => x.LastAction)
                    .Skip((_activePage - 1) * ItemsOnPage)
                    .Take(ItemsOnPage)
                    .ToArrayAsync();
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

                _users = _users.Except(new[] { entity }).ToArray();
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
