using System;
using DataBase;
using DataBase.Entities;
using Telegram.Bot.Types;

namespace Server.Mappers
{
    /// <summary>
    ///     Сопоставляет сущности
    /// </summary>
    internal static class Mapper
    {
        #region Fields

        private static AppDbContext _dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());

        #endregion
        #region Public methods

        //public static UserEntity Map(User user)
        //{
            
        //}

        #endregion
    }
}
