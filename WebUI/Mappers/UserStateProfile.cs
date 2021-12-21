using AutoMapper;
using DataBase.Entities;
using WebUI.Pages.Users.Models;

namespace WebUI.Mappers
{
    /// <summary>
    ///     Профиль для маппинга <see cref="UserStateEntity"/> и <see cref="EditUserStateModel"/>
    /// </summary>
    internal class UserStateProfile : Profile
    {
        #region .ctor

        /// <inheritdoc cref="UserStateProfile"/>
        public UserStateProfile()
        {
            CreateMap<EditUserStateModel, UserStateEntity>();
            CreateMap<UserStateEntity, EditUserStateModel>();
        }


        #endregion
    }
}
