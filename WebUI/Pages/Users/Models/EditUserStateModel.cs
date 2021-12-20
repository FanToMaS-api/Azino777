using DataBase.Models;

namespace WebUI.Pages.Users.Models
{
    /// <summary>
    ///     Модель для редактирования состояния пользователя
    /// </summary>
    public class EditUserStateModel
    {
        /// <summary>
        ///     Баланс пользователя
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        ///     Текущее состояние пользователя
        /// </summary>
        public UserStateType UserStateType { get; set; }

        /// <summary>
        ///     Причина бана
        /// </summary>
        public BanReasonType? BanReason { get; set; }
    }
}
