using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using DataBase.Entities;
using DataBase.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
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

        #endregion

        #region Fields

        private Modal _modalRef;
        private EditUserStateModel _model;
        private UserStateEntity _state = new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Открывает модальное окно
        /// </summary>
        public async Task ShowModalAync(UserStateEntity userState)
        {
            // TODO: AutoMapper, Display()
            // _modalRef = userState;
            _state = userState;
            _modalRef.Show();
        }

        #endregion

        #region Private methods

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
