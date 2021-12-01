using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Entities
{
    /// <summary>
    ///     История игр в блэкджэк
    /// </summary>
    [Table("blackjack_history")]
    public class BlackjackHistoryEntity
    {
        #region Properties

        /// <summary>
        ///     Уникальное id
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
        ///     Состояние игры
        /// </summary>
        [Column("state")]
        public GameStateType GameState { get; set; }

        /// <summary>
        ///     Очки дилера
        /// </summary>
        [Column("dialer_scope")]
        public int DialerScope { get; set; }

        /// <summary>
        ///     Очки пользователя
        /// </summary>
        [Column("user_scope")]
        public int UserScope { get; set; }

        /// <summary>
        ///     Ставка в игре
        /// </summary>
        [Column("bid")]
        public double Bid { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void SetupModelBuilder(ModelBuilder modelBuilder)
        {
            // Ключ
            modelBuilder.Entity<BlackjackHistoryEntity>().HasKey(_ => _.Id);

            // Связи
            modelBuilder.Entity<BlackjackHistoryEntity>()
                .HasOne(_ => _.User)
                .WithMany(_ => _.BlackjackHistory)
                .HasForeignKey(_ => _.UserId);

            // Индексы
            modelBuilder.Entity<BlackjackHistoryEntity>().HasIndex(_ => _.UserId).HasDatabaseName("IX_blackjack_history_user_id");
            modelBuilder.Entity<BlackjackHistoryEntity>().HasIndex(_ => _.GameState).HasDatabaseName("IX_blackjack_history_state");
            modelBuilder.Entity<BlackjackHistoryEntity>().HasIndex(_ => _.DialerScope).HasDatabaseName("IX_blackjack_history_dialer_scope");
            modelBuilder.Entity<BlackjackHistoryEntity>().HasIndex(_ => _.UserScope).HasDatabaseName("IX_blackjack_history_user_scope");
            modelBuilder.Entity<BlackjackHistoryEntity>().HasIndex(_ => _.Bid).HasDatabaseName("IX_blackjack_history_bid");

            // конвертеры
            modelBuilder.Entity<BlackjackHistoryEntity>()
              .Property(_ => _.GameState)
              .HasConversion(
                  v => v.ToString(),
                  v => (GameStateType)Enum.Parse(typeof(GameStateType), v));
        }

        #endregion
    }
}
