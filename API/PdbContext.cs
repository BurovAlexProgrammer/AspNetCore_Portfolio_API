using Microsoft.EntityFrameworkCore;
using WebDAL.Entities;

namespace WebAPI
{
    public class PdbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public PdbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=3000;Database=Portfolio;Username=postgres;Password=123");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Fluent API - highest priority

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