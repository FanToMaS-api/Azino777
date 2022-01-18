using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBase.Entities
{
    /// <summary>
    ///     Таблица реферальных ссылок
    /// </summary>
    [Table("referral_links")]
    public class ReferralLinkEntity
    {
        /// <summary>
        ///     Уникальный id записи
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
        ///     Реферальная ссылка пользователя
        /// </summary>
        [Column("referral_link")]
        public string ReferralLink { get; set; }

        #region Setup

        /// <summary>
        ///     Настройка сущности
        /// </summary>
        public static void Setup(EntityTypeBuilder<ReferralLinkEntity> builder)
        {
            // Ключ
            builder.HasKey(x => x.Id);

            // Индексы
            builder.HasIndex(x => x.Id).IsUnique().HasDatabaseName("IX_referral_links_id");
            builder.HasIndex(x => x.ReferralLink).IsUnique().HasDatabaseName("IX_referral_links_referral_link");

            // Связи
            builder
                .HasOne(_ => _.User)
                .WithOne(_ => _.UserReferralLink)
                .HasForeignKey<ReferralLinkEntity>(_ => _.UserId)
                .OnDelete(DeleteBehavior.Cascade); ;
        }

        #endregion
    }
}
