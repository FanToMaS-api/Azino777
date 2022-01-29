using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBase
{
    /// <summary>
    ///     Design-time фабрика для <see cref="AppDbContext"/>
    /// </summary>
    [UsedImplicitly]
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <inheritdoc />
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
#if DEBUG
            optionsBuilder.UseNpgsql("Server=127.0.0.1;UserID=postgres;Port=35432;Database=Azino;Password=0000;Include Error Detail=true");
#endif
#if !DEBUG
            optionsBuilder.UseNpgsql("Host=postgres;Port=5432;UserID=postgres;Database=Azino;Password=0000;Include Error Detail=true");
#endif
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
