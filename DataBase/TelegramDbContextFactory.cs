using System;
using DataBase.Repositories.Impl;

namespace DataBase
{
    /// <summary>
    ///     Фабрика для <see cref="TelegramDbContext"/>
    /// </summary>
    public static class TelegramDbContextFactory
    {
        /// <summary>
        ///     Создает контекст <see cref="TelegramDbContext"/>
        /// </summary>
        public static TelegramDbContext Create()
        {
            var dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            return new TelegramDbContext(dbContext);
        }
    }
}
