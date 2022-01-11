using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     История игр в рулетку
    /// </summary>
    [Table("roulette_history")]
    public class RouletteHistoryEntity
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
        ///     Оставшиеся в игре монеты
        /// </summary>
        [Column("coin")]
        public double Coin { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void SetupModelBuilder(ModelBuilder modelBuilder)
        {
            // Ключ
            modelBuilder.Entity<RouletteHistoryEntity>().HasKey(_ => _.Id);

            // Связи
            modelBuilder.Entity<RouletteHistoryEntity>()
                .HasOne(_ => _.User)
                .WithMany(_ => _.RouletteHistory)
                .HasForeignKey(_ => _.UserId);

            // Индексы
            modelBuilder.Entity<RouletteHistoryEntity>().HasIndex(_ => _.UserId).HasDatabaseName("IX_roulette_history_user_id");
            modelBuilder.Entity<RouletteHistoryEntity>().HasIndex(_ => _.GameState).HasDatabaseName("IX_roulette_history_state");
            modelBuilder.Entity<RouletteHistoryEntity>().HasIndex(_ => _.Coin).HasDatabaseName("IX_roulette_history_coin");

            // конвертеры
            modelBuilder.Entity<RouletteHistoryEntity>()
               .Property(_ => _.GameState)
               .HasConversion(
                   v => v.ToString(),
                   v => string.IsNullOrEmpty(v) ? GameStateType.IsOver : (GameStateType)Enum.Parse(typeof(GameStateType), v));
        }

        #endregion
    }
}
