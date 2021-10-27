using Games.Interfaces.Game;
using Games.src;
using System.Collections.Generic;
using Games.Impl.Games;
using Games.Impl.MoneyService;
using Games.User;

namespace Games
{
    internal class Program
    {
        private static void Main()
        {
            var user = UserFactory.CreateUser("0", "FanToMas");
            var moneyHandler = new MoneyHandler();
            moneyHandler.AddBalance(user, 50);

            var menu = new Menu(new List<IGame> { new RouletteGame(user), new Blackjack(user) });
            var game = menu.ChooseGame();

            game.StartGame(user.GetBalance());
        }
    }
}
