using Games.src.User;
using Xunit;

namespace GamesTests
{
    public class UserFactoryTests
    {
        [Fact(DisplayName = "ѕроверка создани€ пользовател€ с верными св-ми")]
        public void Create_ReturnUser_CanReturnRightUser()
        {
            var id = "01ER";
            var nickname = "FanToMas";
            var user = UserFactory.CreateUser(id, nickname);

            Assert.Equal(id, user.Id);
            Assert.Equal(nickname, user.Nickname);
            Assert.Equal(0, user.GetBalance());
        }
    }
}
