using System;
using System.Collections.Generic;
using System.Linq;
using Games.Interfaces.Game;
namespace Games.src
{
    /// <summary>
    ///     Меню игр
    /// </summary>
    internal class Menu
    {
        #region Fields

        private readonly List<IGame> _games;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Menu"/>
        public Menu(List<IGame> games)
        {
            _games = games;
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
