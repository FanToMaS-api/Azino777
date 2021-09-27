using Games.Interfaces.Game;
using Games.src;
using Games.src.Games;
using Games.src.User;
using System.Collections.Generic;
using Games.Impl.MoneyService;

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
