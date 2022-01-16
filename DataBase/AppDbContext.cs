using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.NameTranslation;

namespace DataBase
{
    /// <summary>
    ///     Контекст базы данных
    /// </summary>
    public class AppDbContext : DbContext
    {
        #region .ctor

        /// <summary>
        ///     Инициализация контекста базы данных
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        #endregion

        #region Properties

        /// <summary>
        ///     Пользователи бота
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        ///     Таблица реферальных ссылок
        /// </summary>
        public DbSet<ReferralLinkEntity> ReferralLinks { get; set; }

        /// <summary>
        ///     Состояния пользователей
        /// </summary>
        public DbSet<UserStateEntity> UsersStates { get; set; }

        /// <summary>
        ///     История игр в блэкджэк
        /// </summary>
        public DbSet<BlackjackHistoryEntity> BlackjackHistory { get; set; }

        /// <summary>
        ///     История игр в рулетку
        /// </summary>
        public DbSet<RouletteHistoryEntity> RouletteHistory { get; set; } 
        
        /// <summary>
        ///     Пользователи сайта
        /// </summary>
        public DbSet<WebUserEntity> WebUsers { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Проставляем имя поля по умолчанию (snake_case)
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var storeObjectId = StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema());
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(mapper.TranslateMemberName(property.GetColumnName(storeObjectId)));
                }
            }

            // Setup
            UserEntity.Setup(modelBuilder.Entity<UserEntity>());
            ReferralLinkEntity.Setup(modelBuilder.Entity<ReferralLinkEntity>());
            WebUserEntity.Setup(modelBuilder.Entity<WebUserEntity>());
            UserStateEntity.SetupModelBuilder(modelBuilder);
            BlackjackHistoryEntity.SetupModelBuilder(modelBuilder);
            RouletteHistoryEntity.SetupModelBuilder(modelBuilder);
        }

        #endregion
    }
}
