using System;
using System.Data;
using System.Threading.Tasks;
using Diet.Api.Domain;
using Diet.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Diet.Api.Data
{
    public class DietContext : DbContext
    {
        private readonly IPasswordHasher _passwordHasher;

        public DietContext(DbContextOptions options, [FromServices] IPasswordHasher passwordHasher) : base(options)
        {
            _passwordHasher = passwordHasher;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<AccountSetting> AccountSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(b =>
            {
                b.ToTable("Account");
                b.HasKey(e => e.Id);
                b.HasIndex(e => e.Email).IsUnique();
                b.Property(e => e.Email).IsRequired().HasMaxLength(1024);
                b.Property(e => e.Role).IsRequired().HasDefaultValue(Role.RegularUser).HasMaxLength(50);

                b.HasOne(e => e.Setting).WithOne(e => e.Account).HasForeignKey<AccountSetting>(e => e.AccountId);

                b.HasData(new Account
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@string.com",
                    Role = Role.Admin,
                    PasswordHash = _passwordHasher.HashPassword("8MggZmuNbF")
                });
            });

            modelBuilder.Entity<Meal>(b =>
            {
                b.ToTable("Meal");
                b.HasKey(e => e.Id);
                b.Property(e => e.Date).IsRequired();
                b.Property(e => e.Time).IsRequired();
                b.Property(e => e.Text).IsRequired().HasMaxLength(4000);
                b.Property(e => e.Calories).HasColumnType("decimal(6,2)");
                b.Property(e => e.CalorieStatus).IsRequired().HasDefaultValue(false);
            });

            modelBuilder.Entity<AccountSetting>(b =>
            {
                b.ToTable("AccountSetting");
                b.HasKey(e => e.Id);
                b.Property(e => e.TargetCalories).IsRequired().HasColumnType("decimal(6,2)");
            });
        }
    }
}