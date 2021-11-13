using Games.UserFactory;
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
            var phone = "123456789";
            var balance = 50.2;
            var user = UserFactory.CreateUser(id, nickname, phone, balance);

            Assert.Equal(id, user.Id);
            Assert.Equal(nickname, user.Nickname);
            Assert.Equal(phone, user.PhoneNumber);
            Assert.Equal(balance, user.GetBalance());
        }
    }
}
