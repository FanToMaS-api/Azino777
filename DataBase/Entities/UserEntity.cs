using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     Таблица пользователей бота
    /// </summary>
    [Table("users")]
    public class UserEntity
    {
        #region Properties

        /// <summary>
        ///     Уникальный id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        [Column("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        ///     Номер телефона
        /// </summary>
        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Отражает дату последнего действия пользователя
        /// </summary>
        [Column("last_action")]
        public DateTime LastAction { get; init; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<UserEntity> builder)
        {
            // Ключ
            builder.HasKey(_ => _.Id);

            // Индексы
            builder.HasIndex(_ => _.PhoneNumber)
                .IsUnique()
                .HasDatabaseName("IX_users_phone_number");
            builder.HasIndex(_ => _.Nickname)
                .HasDatabaseName("IX_users_nickname");
            builder.HasIndex(_ => _.LastAction)
                .HasDatabaseName("IX_users_last_action");
        }

        #endregion
    }
}
