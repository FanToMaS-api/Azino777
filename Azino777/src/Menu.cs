using System;
using System.Collections.Generic;
using System.Linq;
using Games.Games;
using Games.Games.Impl;
using Games.Services;
using Games.User;

namespace Games
{
    /// <summary>
    ///     Меню игр
    /// </summary>
    internal class Menu
    {
        #region Fields

        private readonly List<IGame> _games;

        private readonly InOutHandlerBase _inOutHandler;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Menu"/>
        public Menu(IUser user, InOutHandlerBase inOutHandler)
        {
            _inOutHandler = inOutHandler;
            _games = new List<IGame> { new RouletteGame(user, _inOutHandler), new Blackjack(user, _inOutHandler) };
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
