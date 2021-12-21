using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using DataBase.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using WebUI.Pages.Users.Models;

namespace WebUI.Pages.Users.Modals
{
    /// <summary>
    ///     Модалка для отображения состояния пользователя
    /// </summary>
    public partial class UserStateViewModal
    {
        #region Fields

        private Modal _modalRef;

        private UserStateEntity _model = new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Открывает модальное окно
        /// </summary>
        public void ShowModal(UserStateEntity userState)
        {
            _model = userState;
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
