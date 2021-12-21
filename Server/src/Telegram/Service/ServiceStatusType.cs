namespace Server.Telegram.Service
{
    /// <summary>
    ///     Отражает статус сервиса
    /// </summary>
    public enum ServiceStatusType
    {
        /// <summary>
        ///     Не запущен
        /// </summary>
        NotLaunch,

        /// <summary>
        ///     Запущен
        /// </summary>
        Running,
        
        /// <summary>
        ///     Остановлен
        /// </summary>
        Stopped
    }
}
