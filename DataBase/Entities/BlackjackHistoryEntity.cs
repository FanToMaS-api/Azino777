using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        public static void Setup(EntityTypeBuilder<BlackjackHistoryEntity> builder)
        {
            // Ключ
            builder.HasKey(_ => _.Id);

            // Индексы
            builder.HasIndex(_ => _.UserId).HasDatabaseName("IX_blackjack_history_user_id");
            builder.HasIndex(_ => _.GameState).HasDatabaseName("IX_blackjack_history_state");
            builder.HasIndex(_ => _.DialerScope).HasDatabaseName("IX_blackjack_history_dialer_scope");
            builder.HasIndex(_ => _.UserScope).HasDatabaseName("IX_blackjack_history_user_scope");
            builder.HasIndex(_ => _.Bid).HasDatabaseName("IX_blackjack_history_bid");
        }

        #endregion
    }
}
