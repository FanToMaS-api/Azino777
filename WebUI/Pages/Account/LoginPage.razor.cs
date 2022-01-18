using Microsoft.AspNetCore.Components;

namespace WebUI.Pages.Account
{
    /// <summary>
    ///     Страница входа
    /// </summary>
    public partial class LoginPage
    {
        #region Injects

        /// <inheritdoc cref="NavigationManager"/>
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        #endregion

        #region Fields

        private AuthorizationModel _model = new();

        private string _errorMessage = string.Empty;

        #endregion

        #region Public methods

        /// <summary>
        ///     Осуществляет шифрование введенных данных и пересылкой их контроллеру входа
        /// </summary>
        public void Authorization()
        {
            if (string.IsNullOrEmpty(_model.Name) || string.IsNullOrEmpty(_model.Password))
            {
                _errorMessage = "Uncorrect login or passowrd";
                StateHasChanged();

                return;
            }

            var uri = $"/login?data={_model.Serialize()}";

            NavigationManager.NavigateTo(uri, true);
        }

        #endregion
    }
}
