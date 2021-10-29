using System;
using System.Collections.Generic;
using System.Linq;
using Games.Impl.Games;
using Games.Impl.OutputHandlers;
using Games.Interfaces.Game;
using Games.Interfaces.User;

namespace Games.src
{
    /// <summary>
    ///     Меню игр
    /// </summary>
    internal class Menu
    {
        #region Fields

        private readonly List<IGame> _games;

        private readonly OutputHandlerBase _outputHandler;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Menu"/>
        public Menu(IUser user, OutputHandlerBase outputHandler)
        {
            _outputHandler = outputHandler;
            _games = new List<IGame> { new RouletteGame(user, _outputHandler), new Blackjack(user, _outputHandler) };
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает случайную игру
        /// </summary>
        public IGame ChooseGame()
        {
            return _games.ElementAt(new Random().Next(0, _games.Count));
        }

        #endregion
    }
}
