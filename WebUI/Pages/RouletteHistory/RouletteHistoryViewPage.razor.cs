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

namespace WebUI.Pages.RouletteHistory
{
    /// <summary>
    ///     Страница истории игр в австралийскую рулетку
    /// </summary>
    public partial class RouletteHistoryViewPage
    {
        #region Injects

        /// <inheritdoc cref="IServiceScopeFactory" />
        [Inject]
        private IServiceScopeFactory Scope { get; set; }

        #endregion

        #region Fields

        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        private RouletteHistoryEntity[] _games = Array.Empty<RouletteHistoryEntity>();

        private int _totalCount;

        private int _activePage;

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
        ///     Кол-во игр на 1ой странице
        /// </summary>
        private const int ItemsOnPage = 40;

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            using var scope = Scope.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            _totalCount = database.BotUsers.CreateQuery().Count();

            await RefreshAsync();
        }

        /// <inheritdoc />
        protected override async Task OnLocationChangedAsync() => await RefreshAsync();

        /// <summary>
        ///     Получение список игр из бд
        /// </summary>
        private async Task RefreshAsync()
        {
            try
            {
                using var scope = Scope.CreateScope();
                using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();

                _activePage = NavigationManager.TryGetQueryParameter(PageNumberQueryParameter, out var res, 1) ? res : 1;
                var queryable = database.RouletteHistory
                    .CreateQuery()
                    .OrderByDescending(x => x.GameState)
                    .Skip((_activePage - 1) * ItemsOnPage)
                    .Take(ItemsOnPage);


                queryable = queryable.Include(_ => _.User);
                if (NavigationManager.TryGetQueryParameter<string>(UserNameQueryParameter, out var filterName))
                {
                    queryable = queryable.Where(_ => _.User.FirstName.Contains(filterName));
                }

                _games = await queryable.ToArrayAsync();
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

        #endregion
    }
}
