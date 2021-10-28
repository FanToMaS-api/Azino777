using Games.Interfaces.Game;
using Games.src;
using System.Collections.Generic;
using Games.Impl.Games;
using Games.Impl.MoneyService;

namespace Games
{
    internal class Program
    {
        private static void Main()
        {
            var user = UserFactory.UserFactory.CreateUser("0", "FanToMas", "123456789");
            var moneyHandler = new MoneyHandler();
            moneyHandler.AddBalance(user, 50);

            var menu = new Menu(new List<IGame> { new RouletteGame(user), new Blackjack(user) });
            var game = menu.ChooseGame();

            game.StartGameAsync(user.GetBalance());
        }
    }
}
