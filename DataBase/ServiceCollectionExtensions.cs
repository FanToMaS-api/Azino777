using System;
using System.Configuration;
using DataBase.Repositories;
using DataBase.Repositories.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace DataBase
{
    /// <summary>
    ///     Расширение для сервисов
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        #region Fields

        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        private const string DatabaseConnectionEnvName = "POSTGRES_CONNECTION_STRING";

        #endregion

        #region Public methods

        /// <summary>
        ///     Регистрация DbContext как сервиса
        /// </summary>
        public static void UsePostgresDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(DatabaseConnectionEnvName);
            Logger.Trace($"Db connection string: '{connectionString}'");

            services.AddDbContext<AppDbContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<ITelegramDbContext, TelegramDbContext>();
        }

        /// <summary>
        ///     Применение миграций базы данных
        /// </summary>
        public static void ApplyMigrations(this IServiceProvider serviceProvider)
        {
            try
            {
                Logger.Debug("Applying database migrations...");

                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<AppDbContext>();

                db.Database.Migrate();

                Logger.Debug("Database migrations successfully applied");
            }
            catch (Exception ex)
            {
                throw new Exception("Error applying database migrations", ex);
            }
        }

        #endregion
    }
}
