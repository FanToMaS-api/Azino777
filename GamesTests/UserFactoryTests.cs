using Games.UserFactory;
using Xunit;

namespace GamesTests
{
    public class UserFactoryTests
    {
        [Fact(DisplayName = "ѕроверка создани€ пользовател€ с верными св-ми")]
        public void Create_ReturnUser_CanReturnRightUser()
        {
            var id = 132;
            var nickname = "FanToMas";
            var balance = 50.2;
            var chatId = 12314;
            var user = UserFactory.CreateUser(id, id, chatId, nickname, balance);

            Assert.Equal(id, user.Id);
            Assert.Equal(nickname, user.Nickname);
            Assert.Equal(balance, user.GetBalance());
            Assert.Equal(balance, user.ChatId);
        }
    }
}
