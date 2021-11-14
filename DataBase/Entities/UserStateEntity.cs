using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     Таблица состояний пользователей бота
    /// </summary>
    [Table("users_state")]
    public class UserStateEntity
    {
        #region Properties

        /// <summary>
        ///     Уникальное id состояния пользователя
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Id пользователя
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }

        /// <summary>
        ///     Сам пользователь для связи
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        ///     Баланс пользователя
        /// </summary>
        [Column("balance")]
        public double Balance { get; set; }

        /// <summary>
        ///     Текущее состояние пользователя
        /// </summary>
        [Column("state_type")]
        public UserStateType UserStateType { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<UserStateEntity> builder)
        {
            // Ключ
            builder.HasKey(_ => _.Id);

            // Связи
            builder
                .HasOne(_ => _.User)
                .WithOne(_ => _.UserState)
                .HasForeignKey<UserStateEntity>(_ => _.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            builder.HasIndex(_ => _.UserId)
                .IsUnique()
                .HasDatabaseName("IX_users_state_user_id");
            builder.HasIndex(_ => _.UserStateType)
                .HasDatabaseName("IX_users_state_state_type");
            builder.HasIndex(_ => _.Balance)
                .HasDatabaseName("IX_users_state_balance");
        }

        #endregion
    }
}
