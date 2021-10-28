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
            var user = UserFactory.CreateUser(id, nickname, phone);

            Assert.Equal(id, user.Id);
            Assert.Equal(nickname, user.Nickname);
            Assert.Equal(phone, user.PhoneNumber);
            Assert.Equal(0, user.GetBalance());
        }
    }
}
