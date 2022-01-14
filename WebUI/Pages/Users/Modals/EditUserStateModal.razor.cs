using System;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using DataBase.Entities;
using DataBase.Repositories;
using DataBase.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using WebUI.Pages.Users.Models;

namespace WebUI.Pages.Users.Modals
{
    /// <summary>
    ///     Модалка для редактирования состояния пользователя
    /// </summary>
    public partial class EditUserStateModal
    {
        #region Injects

        /// <inheritdoc cref="IServiceScopeFactory" />
        [Inject]
        private IServiceScopeFactory Scope { get; set; }

        /// <inheritdoc cref="IMapper" />
        [Inject]
        private IMapper Mapper { get; set; }

        #endregion

        #region Fields

        private ILogger Logger = LogManager.GetCurrentClassLogger();

        private Modal _modalRef;

        private EditUserStateModel _model = new();

        private UserStateType[] _userStates = Enum.GetValues<UserStateType>();

        private BanReasonType[] _banReasons = Enum.GetValues<BanReasonType>();

        #endregion

        #region Public methods

        /// <summary>
        ///     Открывает модальное окно
        /// </summary>
        public void ShowModal(UserStateEntity userState)
        {
            _model = Mapper.Map<EditUserStateModel>(userState);
            _modalRef.Show();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Сохраняет изменения
        /// </summary>
        private async Task SaveAsync()
        {
            using var scope = Scope.CreateScope();
            using var database = scope.ServiceProvider.GetRequiredService<ITelegramDbContext>();
            try
            {
                await database.UserStates.UpdateAsync(_model.Id, UpdateUserState);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Error while updating UserStateEntity");
            }
            finally
            {
                Close();
                await InvokeAsync(StateHasChanged);
            }

            void UpdateUserState(UserStateEntity entity)
            {
                entity.Balance = _model.Balance;
                entity.UserStateType = _model.UserStateType;
                entity.BanReason = _model.BanReason;
                entity.WarningNumber = _model.WarningNumber;
            }
        }

        /// <summary>
        ///     Закрывает модальное окно
        /// </summary>
        private void Close()
        {
            _modalRef.Hide();
        }

        #endregion
    }
}
