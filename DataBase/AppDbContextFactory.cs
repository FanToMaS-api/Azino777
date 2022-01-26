using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBase
{
    /// <summary>
    ///     Design-time фабрика для <see cref="AppDbContext"/>
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <inheritdoc />
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Server=0.0.0.0;Host=postgres;Port=5432;Database=Azino;Username=postgres;Password=0000");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
