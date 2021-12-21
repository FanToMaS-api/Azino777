﻿using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using DataBase.Entities;
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

        /// <inheritdoc cref="IMapper" />
        [Inject]
        private IMapper Mapper { get; set; }

        #endregion

        #region Fields

        private Modal _modalRef;

        private EditUserStateModel _model = new();

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
        private void HandleValidSubmit()
        {
            
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