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
            optionsBuilder.UseNpgsql("Server=127.0.0.1;User Id=postgres;Port=35432;Database=Azino;Password=0000;Include Error Detail=true");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
