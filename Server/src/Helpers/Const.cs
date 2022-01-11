using System;

namespace Server.Helpers
{
    /// <summary>
    ///     Константы, используемые в боте
    /// </summary>
    internal static class Const
    {
        #region Properties

        /// <summary>
        ///     Награда пользователю, если его реферал выйграл игру
        /// </summary>
        public const int ReferralAward = 5;

        /// <summary>
        ///     Кол-во предупреждений пользователю, после которых он получит бан
        /// </summary>
        public const int LimitWarningNumber = 3;

        /// <summary>
        ///     Считать спамом все, что пришло быстрее чем
        /// </summary>
        public static TimeSpan TimeToDefineSpam = TimeSpan.FromSeconds(1);

        #endregion
    }
}
