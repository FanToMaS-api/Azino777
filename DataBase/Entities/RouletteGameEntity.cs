using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     История игр в рулетку
    /// </summary>
    [Table("roulette_games")]
    internal class RouletteGameEntity
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
        public static void Setup(EntityTypeBuilder<RouletteGameEntity> builder)
        {
            // Ключ
            builder.HasKey(_ => _.Id);

            // Индексы
            builder.HasIndex(_ => _.UserId).HasDatabaseName("IX_roulette_games_user_id");
            builder.HasIndex(_ => _.GameState).HasDatabaseName("IX_roulette_games_state");
            builder.HasIndex(_ => _.Coin).HasDatabaseName("IX_roulette_games_coin");
        }

        #endregion
    }
}
