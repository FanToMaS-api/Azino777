using Microsoft.EntityFrameworkCore;

namespace DataBase
{
    /// <summary>
    ///     Контекст базы данных
    /// </summary>
    internal class AppDbContext : DbContext
    {
        #region Properties



        #endregion

        #region .ctor

        /// <summary>
        ///     Инициализация контекста базы данных
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        #endregion
    }
}
