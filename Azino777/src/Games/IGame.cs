using System.Threading;
using System.Threading.Tasks;
using Games.Services;

namespace Games.Games
{
    /// <summary>
    ///     Базовый интерфейс игры
    /// </summary>
    public interface IGame
    {
        #region Properties

        /// <summary>
        ///     Обработчик вывода сообщений в разные среды
        /// </summary>
        InOutHandlerBase InOutHandler { get; }

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
        public Task StartGameAsync(double bid, CancellationToken token = default);

        /// <summary>
        ///     Логика игры
        /// </summary>
        public Task LogicAsync(string input, CancellationToken token);

        /// <summary>
        ///     Определяет, когда игра завершена
        /// </summary>
        public Task<bool> GameOverAsync(CancellationToken token);

        /// <summary>
        ///     Обрабатывает завершение игры
        /// </summary>
        public Task<double> EndGameAsync(CancellationToken token);

        /// <summary>
        ///     Выводит информацию об игре
        /// </summary>
        public string ToString();

        #endregion
    }
}
