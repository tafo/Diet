﻿// <auto-generated />
using System;
using Diet.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Diet.Api.Data.Migrations
{
    [DbContext(typeof(DietContext))]
    [Migration("20200207132651_CreateAccountSetting")]
    partial class CreateAccountSetting
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Diet.Api.Domain.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(1024)")
                        .HasMaxLength(1024);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50)
                        .HasDefaultValue("RegularUser");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Account");

                    b.HasData(
                        new
                        {
                            Id = new Guid("172b12e2-8ea5-4011-8960-ee71c0fe5940"),
                            Email = "admin@domain.com",
                            PasswordHash = "AAAAAAEAAAAQHfgmjF/Kcz5Hpn5d2peYbyAaEMC7PA49pHkx4M46XH8K7XUvt34EAsJzYwbo9kZt",
                            Role = "Admin"
                        });
                });

            modelBuilder.Entity("Diet.Api.Domain.AccountSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("TargetCalories")
                        .HasColumnType("decimal(6,2)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("AccountSetting");
                });

            modelBuilder.Entity("Diet.Api.Domain.Meal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("Calories")
                        .HasColumnType("decimal(6,2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000);

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Meal");
                });

            modelBuilder.Entity("Diet.Api.Domain.AccountSetting", b =>
                {
                    b.HasOne("Diet.Api.Domain.Account", "Account")
                        .WithOne("Setting")
                        .HasForeignKey("Diet.Api.Domain.AccountSetting", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Diet.Api.Domain.Meal", b =>
                {
                    b.HasOne("Diet.Api.Domain.Account", "Account")
                        .WithMany("Meals")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}