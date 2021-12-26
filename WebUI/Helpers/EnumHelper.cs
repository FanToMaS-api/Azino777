using DataBase.Models;

namespace WebUI.Helpers
{
    /// <summary>
    ///     Расширение Enum
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        ///     Отобразить
        /// </summary>
        public static string Display(this UserStateType userState)
        {
            return userState switch
            {
                UserStateType.Active => "Активный",
                UserStateType.Inactive => "Неактивный",
                UserStateType.Banned => "Забанен",
                UserStateType.BlockedBot => "Заблокировал бота",
                _ => userState.ToString()
            };
        }

        /// <summary>
        ///     Отобразить
        /// </summary>
        public static string Display(this BanReasonType banReason)
        {
            return banReason switch
            {
                BanReasonType.NotBanned => "Не забанен",
                BanReasonType.Common => "Общая",
                BanReasonType.BadBehavior => "Нарушил правила",
                BanReasonType.Spam => "Спам",
                _ => banReason.ToString()
            };
        }

        /// <summary>
        ///     Отобразить
        /// </summary>
        public static string Display(this GameStateType stateType)
        {
            return stateType switch
            {
                GameStateType.IsOn => "Активна",
                GameStateType.IsOver => "Закончена",
                _ => stateType.ToString()
            };
        }
    }
}
