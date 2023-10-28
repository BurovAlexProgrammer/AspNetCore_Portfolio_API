using Microsoft.EntityFrameworkCore;
using WebDAL.Entities;

namespace WebAPI
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=3000;Database=Portfolio;Username=postgres;Password=123");
        }

        //Fluent API - highest priority
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.Entity<Guest>()
            //     .ToTable("WTF") //Table name
            //     .HasKey("Id"); //Primary key

            // builder.Entity<Department>()
            //     .Property(x => x.Name)
            //     .IsRequired()
            //     .HasColumnName("DepName");
        }
    }
}