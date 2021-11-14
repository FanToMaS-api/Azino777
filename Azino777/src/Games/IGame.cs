using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Services;
using Games.User;

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
        IInOutHandler InOutHandler { get; }

        /// <summary>
        ///     Событие при обновлении статуса игры
        /// </summary>
        public event Func<IGame, EventArgs, CancellationToken, Task> OnGameUpdated;

        /// <summary>
        ///     Событие при завершении игры
        /// </summary>
        public event Func<IGame, EventArgs, CancellationToken, Task> OnGameEnded;

        /// <summary>
        ///     Пользователь бота, играющий в эту игру
        /// </summary>
        public IUser User { get; }

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
        public bool IsGameOver();

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
