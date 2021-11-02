namespace UserServer.src.Services
{
    /// <summary>
    ///     Отчевает за работу с монетами пользователя
    /// </summary>
    internal class MoneyHandler : IMoneyHandler
    {
        #region Fields

        #endregion

        #region .ctor

        /// <inheritdoc cref="MoneyHandler"/>
        public MoneyHandler()
        {

        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void AddBalance(IUser user, double value)
        {
            user.State.AddBalance(value);
        }

        #endregion
    }
}
