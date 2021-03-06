using DataBase.Types;

namespace WebUI.Pages.Users.Models
{
    /// <summary>
    ///     Модель для редактирования состояния пользователя
    /// </summary>
    public class EditUserStateModel
    {
        /// <summary>
        ///     Уникальный id состояния
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Баланс пользователя
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        ///     Кол-во предупреждений
        /// </summary>
        public int WarningNumber { get; set; }

        /// <summary>
        ///     Текущее состояние пользователя
        /// </summary>
        public UserStateType UserStateType { get; set; }

        /// <summary>
        ///     Причина бана
        /// </summary>
        public BanReasonType BanReason { get; set; }
    }
}
