using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     Таблица состояний пользователей бота
    /// </summary>
    [Table("bot_users_state")]
    public class BotUserStateEntity
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
        public BotUserEntity User { get; set; }

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

        /// <summary>
        ///     Причина бана
        /// </summary>
        [Column("ban_reason")]
        public BanReasonType BanReason { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void SetupModelBuilder(ModelBuilder modelBuilder)
        {
            // Ключ
            modelBuilder.Entity<BotUserStateEntity>().HasKey(_ => _.Id);

            // Связи
            modelBuilder.Entity<BotUserStateEntity>()
                .HasOne(_ => _.User)
                .WithOne(_ => _.UserState)
                .HasForeignKey<BotUserStateEntity>(_ => _.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            modelBuilder.Entity<BotUserStateEntity>().HasIndex(_ => _.UserId).IsUnique().HasDatabaseName("IX_bot_users_state_user_id");
            modelBuilder.Entity<BotUserStateEntity>().HasIndex(_ => _.UserStateType).HasDatabaseName("IX_bot_users_state_state_type");
            modelBuilder.Entity<BotUserStateEntity>().HasIndex(_ => _.Balance).HasDatabaseName("IX_bot_users_state_balance");
            modelBuilder.Entity<BotUserStateEntity>().HasIndex(_ => _.BanReason).HasDatabaseName("IX_bot_users_state_ban_reason");

            // конвертеры
            modelBuilder.Entity<BotUserStateEntity>()
                .Property(_ => _.UserStateType)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrEmpty(v) ? UserStateType.Active : (UserStateType)Enum.Parse(typeof(UserStateType), v));

            modelBuilder.Entity<BotUserStateEntity>()
                .Property(_ => _.BanReason)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrEmpty(v) ? BanReasonType.NotBanned : (BanReasonType)Enum.Parse(typeof(BanReasonType), v));
        }

        #endregion
    }
}
