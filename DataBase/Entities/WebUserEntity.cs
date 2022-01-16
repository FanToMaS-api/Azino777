using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     Сущность пользователя имеющего доступ к сайту управления ботом
    /// </summary>
    [Table("web_users")]
    public class WebUserEntity
    {
        #region Properties

        /// <summary>
        ///     Уникальный идентификатор
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        [Required]
        [Column("username")]
        public string Username { get; set; }

        /// <summary>
        ///     Пароль пользователя
        /// </summary>
        [Required]
        [Column("password")]
        public string Password { get; set; }

        /// <summary>
        ///     Время создания записи
        /// </summary>
        [Column("created")]
        public DateTime Created { get; set; }

        /// <summary>
        ///     Время обновления записи в базе данных
        /// </summary>
        [Column("updated")]
        public DateTime Updated { get; set; }

        /// <summary>
        ///     Роль пользователя
        /// </summary>
        [Column("role")]
        public WebUserRoleType Role { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<WebUserEntity> builder)
        {
            // индексы
            builder.HasIndex(_ => _.Id).HasDatabaseName("IX_web_users_id");
            builder.HasIndex(_ => _.Username).IsUnique().HasDatabaseName("IX_web_users_username");
            builder.HasIndex(_ => _.Password).HasDatabaseName("IX_web_users_password");
            builder.HasIndex(_ => _.Created).HasDatabaseName("IX_web_users_created");
            builder.HasIndex(_ => _.Updated).HasDatabaseName("IX_web_users_updated");
            builder.HasIndex(_ => _.Role).HasDatabaseName("IX_web_users_role");

        }

        #endregion
    }
}
