﻿// <auto-generated />
using System;
using DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBase.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220117192544_AddWebUserSessionTable")]
    partial class AddWebUserSessionTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("DataBase.Entities.BlackjackHistoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Bid")
                        .HasColumnType("double precision")
                        .HasColumnName("bid");

                    b.Property<int>("DialerScope")
                        .HasColumnType("integer")
                        .HasColumnName("dialer_scope");

                    b.Property<string>("GameState")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<int>("UserScope")
                        .HasColumnType("integer")
                        .HasColumnName("user_scope");

                    b.HasKey("Id");

                    b.HasIndex("Bid")
                        .HasDatabaseName("IX_blackjack_history_bid");

                    b.HasIndex("DialerScope")
                        .HasDatabaseName("IX_blackjack_history_dialer_scope");

                    b.HasIndex("GameState")
                        .HasDatabaseName("IX_blackjack_history_state");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_blackjack_history_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_blackjack_history_user_id");

                    b.HasIndex("UserScope")
                        .HasDatabaseName("IX_blackjack_history_user_scope");

                    b.ToTable("blackjack_history");
                });

            modelBuilder.Entity("DataBase.Entities.ReferralLinkEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ReferralLink")
                        .HasColumnType("text")
                        .HasColumnName("referral_link");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_referral_links_id");

                    b.HasIndex("ReferralLink")
                        .IsUnique()
                        .HasDatabaseName("IX_referral_links_referral_link");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("referral_links");
                });

            modelBuilder.Entity("DataBase.Entities.RouletteHistoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Coin")
                        .HasColumnType("double precision")
                        .HasColumnName("coin");

                    b.Property<string>("GameState")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("Coin")
                        .HasDatabaseName("IX_roulette_history_coin");

                    b.HasIndex("GameState")
                        .HasDatabaseName("IX_roulette_history_state");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_roulette_history_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_roulette_history_user_id");

                    b.ToTable("roulette_history");
                });

            modelBuilder.Entity("DataBase.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<string>("FirstName")
                        .HasColumnType("text")
                        .HasColumnName("firstname");

                    b.Property<DateTime>("LastAction")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_action");

                    b.Property<string>("LastName")
                        .HasColumnType("text")
                        .HasColumnName("lastname");

                    b.Property<string>("Nickname")
                        .HasColumnType("text")
                        .HasColumnName("nickname");

                    b.Property<string>("ReferralLink")
                        .HasColumnType("text")
                        .HasColumnName("referral_link");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.HasKey("Id");

                    b.HasIndex("ChatId")
                        .IsUnique()
                        .HasDatabaseName("IX_users_chat_id");

                    b.HasIndex("FirstName")
                        .HasDatabaseName("IX_users_firstname");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_users_id");

                    b.HasIndex("LastAction")
                        .HasDatabaseName("IX_users_last_action");

                    b.HasIndex("LastName")
                        .HasDatabaseName("IX_users_lastname");

                    b.HasIndex("Nickname")
                        .HasDatabaseName("IX_users_nickname");

                    b.HasIndex("ReferralLink")
                        .HasDatabaseName("IX_users_referral_link");

                    b.HasIndex("TelegramId")
                        .IsUnique()
                        .HasDatabaseName("IX_users_telegram_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("DataBase.Entities.UserStateEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Balance")
                        .HasColumnType("double precision")
                        .HasColumnName("balance");

                    b.Property<string>("BanReason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("ban_reason");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<string>("UserStateType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state_type");

                    b.Property<int>("WarningNumber")
                        .HasColumnType("integer")
                        .HasColumnName("warning_number");

                    b.HasKey("Id");

                    b.HasIndex("Balance")
                        .HasDatabaseName("IX_users_state_balance");

                    b.HasIndex("BanReason")
                        .HasDatabaseName("IX_users_state_ban_reason");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_users_state_id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("IX_users_state_user_id");

                    b.HasIndex("UserStateType")
                        .HasDatabaseName("IX_users_state_state_type");

                    b.HasIndex("WarningNumber")
                        .HasDatabaseName("IX_users_state_warning_number");

                    b.ToTable("users_state");
                });

            modelBuilder.Entity("DataBase.Entities.WebUserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.HasIndex("Created")
                        .HasDatabaseName("IX_web_users_created");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_web_users_id");

                    b.HasIndex("Password")
                        .HasDatabaseName("IX_web_users_password");

                    b.HasIndex("Role")
                        .HasDatabaseName("IX_web_users_role");

                    b.HasIndex("Updated")
                        .HasDatabaseName("IX_web_users_updated");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("IX_web_users_username");

                    b.ToTable("web_users");
                });

            modelBuilder.Entity("DataBase.Entities.WebUserSessionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("Expired")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("expired");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("Expired")
                        .HasDatabaseName("IX_web_user_sessions_expired");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_web_user_sessions_id");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_web_user_sessions_status");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_web_user_sessions_user_id");

                    b.ToTable("web_user_sessions");
                });

            modelBuilder.Entity("DataBase.Entities.BlackjackHistoryEntity", b =>
                {
                    b.HasOne("DataBase.Entities.UserEntity", "User")
                        .WithMany("BlackjackHistory")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataBase.Entities.ReferralLinkEntity", b =>
                {
                    b.HasOne("DataBase.Entities.UserEntity", "User")
                        .WithOne("UserReferralLink")
                        .HasForeignKey("DataBase.Entities.ReferralLinkEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataBase.Entities.RouletteHistoryEntity", b =>
                {
                    b.HasOne("DataBase.Entities.UserEntity", "User")
                        .WithMany("RouletteHistory")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataBase.Entities.UserStateEntity", b =>
                {
                    b.HasOne("DataBase.Entities.UserEntity", "User")
                        .WithOne("UserState")
                        .HasForeignKey("DataBase.Entities.UserStateEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataBase.Entities.WebUserSessionEntity", b =>
                {
                    b.HasOne("DataBase.Entities.WebUserEntity", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataBase.Entities.UserEntity", b =>
                {
                    b.Navigation("BlackjackHistory");

                    b.Navigation("RouletteHistory");

                    b.Navigation("UserReferralLink");

                    b.Navigation("UserState");
                });

            modelBuilder.Entity("DataBase.Entities.WebUserEntity", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}