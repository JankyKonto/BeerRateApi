using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace BeerRateApi
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Beer> Beers { get; set; }
        public DbSet<BeerImage> BeerImages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<User>()
            .Property(d => d.UserType)
            .HasConversion<string>();
        }
    }
}
