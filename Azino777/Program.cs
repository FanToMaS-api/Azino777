using Games.Impl.MoneyService;
using Games.Impl.Services;
using Games.src;

namespace Games
{
    internal class Program
    {
        private static void Main()
        {
            var user = UserFactory.UserFactory.CreateUser("0", "FanToMas", "123456789");
            var moneyHandler = new MoneyHandler();
            moneyHandler.AddBalance(user, 50);
            var consoleOutputHandler = new ConsoleInOutHandler();

            var menu = new Menu(user, consoleOutputHandler);
            var game = menu.ChooseGame();

            game.StartGameAsync(user.GetBalance());
        }
    }
}
