using Newtonsoft.Json;

namespace WebUI.Pages.Account
{
    /// <summary>
    ///     Модель авторизации пользователя
    /// </summary>
    public class AuthorizationModel
    {
        #region Properties

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Токен пользователя
        /// </summary>
        public string Password { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Сериализация модели
        /// </summary>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        public static AuthorizationModel Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<AuthorizationModel>(json);
        }

        #endregion
    }
}
