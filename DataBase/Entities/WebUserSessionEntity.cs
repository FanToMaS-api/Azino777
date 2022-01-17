using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     Сущность сессии пользоватлея сайта
    /// </summary>
    [Table("web_user_sessions")]
    public class WebUserSessionEntity
    {
        #region Properties

        /// <summary>
        ///     Идентификатор сессии
        /// </summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Идентификатор пользователя
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }

        /// <summary>
        ///     Сущность пользователя для связи
        /// </summary>
        public WebUserEntity User { get; set; }

        /// <summary>
        ///     Дата экспирации сессии
        /// </summary>
        [Column("expired")]
        public DateTime? Expired { get; set; }

        /// <summary>
        ///     Статус сессии пользователя
        /// </summary>
        [Column("status")]
        public WebUserSessionType Status { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        internal static void Setup(EntityTypeBuilder<WebUserSessionEntity> builder)
        {
            // индексы
            builder.HasIndex(t => t.Id).IsUnique().HasDatabaseName("IX_web_user_sessions_id");
            builder.HasIndex(t => t.UserId).HasDatabaseName("IX_web_user_sessions_user_id");
            builder.HasIndex(t => t.Expired).HasDatabaseName("IX_web_user_sessions_expired");
            builder.HasIndex(t => t.Status).HasDatabaseName("IX_web_user_sessions_status");

            // конвертеры
            builder.Property(_ => _.Status)
                .HasConversion(
                _ => _.ToString(),
                _ => string.IsNullOrEmpty(_) ? WebUserSessionType.Undefined : Enum.Parse<WebUserSessionType>(_));
        }

        #endregion
    }
}
