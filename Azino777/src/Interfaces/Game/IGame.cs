namespace Games.Interfaces.Game
{
    /// <summary>
    ///     Базовый интерфейс игры
    /// </summary>
    public interface IGame
    {
        #region Properties

        /// <summary>
        ///     Название игры
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///      Описание игры
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Правила игры
        /// </summary>
        public string GameRules { get; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Начало игры
        /// </summary>
        /// <param name="bid"> Ставка </param>
        public void StartGame(double bid);

        /// <summary>
        ///     Логика игры
        /// </summary>
        public void Logic(string input);

        /// <summary>
        ///     Определяет, когда игра завершена
        /// </summary>
        public bool GameOver();

        /// <summary>
        ///     Обрабатывает завершение игры
        /// </summary>
        public double EndGame();

        /// <summary>
        ///     Выводит информацию об игре
        /// </summary>
        public string ToString();

        #endregion
    }
}
