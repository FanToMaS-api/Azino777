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

        private readonly IUser _user;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Menu"/>
        public Menu(IUser user, InOutHandlerBase inOutHandler)
        {
            _inOutHandler = inOutHandler;
            _user = user;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает случайную игру
        /// </summary>
        public IGame ChooseGame()
        {
            var gameNumber = new Random().Next(1, 2);
            switch (gameNumber)
            {
                case 0:
                    {
                        return new RouletteGame(_user, _inOutHandler);
                    }
                case 1:
                    {
                        return new Blackjack(_user, _inOutHandler);
                    }

                default: throw new ArgumentException();
            }
        }

        #endregion
    }
}
