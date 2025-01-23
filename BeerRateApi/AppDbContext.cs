using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace BeerRateApi
{
    /// <summary>
    /// Represents the database context for the BeerRate application.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the collection of users in the database.
        /// </summary>
        /// <value>The collection of users.</value>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Gets or sets the collection of reviews in the database.
        /// </summary>
        /// <value>The collection of reviews.</value>
        public DbSet<Review> Reviews { get; set; }
        /// <summary>
        /// Gets or sets the collection of beers in the database.
        /// </summary>
        /// <value>The collection of beers.</value>
        public DbSet<Beer> Beers { get; set; }
        /// <summary>
        /// Gets or sets the collection of beer images in the database.
        /// </summary>
        /// <value>The collection of beer images.</value>
        public DbSet<BeerImage> BeerImages { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the database context.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }

        /// <summary>
        /// Configures the model for the database context.
        /// </summary>
        /// <param name="modelBuilder">The model builder to configure the entity relationships and properties.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<User>()
            .Property(d => d.UserType)
            .HasConversion<string>();
        }
    }
}
