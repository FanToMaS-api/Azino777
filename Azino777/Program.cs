using Games.Services;

namespace Games
{
    internal class Program
    {
        private static void Main()
        {
            var user = UserFactory.UserFactory.CreateUser("0", "FanToMas", "123456789", 50);
            var consoleOutputHandler = new ConsoleInOutHandler();

            var menu = new Menu(user, consoleOutputHandler);
            var game = menu.ChooseGame();

            game.StartGameAsync(user.GetBalance());
        }
    }
}
