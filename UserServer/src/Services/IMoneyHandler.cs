using Games.User;

namespace UserServer.src.Services
{
    /// <summary>
    ///     Базовый интерфейс для обработчика баланса клиента
    /// </summary>
    public interface IMoneyHandler
    {
        #region Properties



        #endregion

        #region Public methods

        /// <summary>
        ///     Пополняет баланс пользователя
        /// </summary>
        public void AddBalance(IUser user, double value);

        #endregion
    }
}
