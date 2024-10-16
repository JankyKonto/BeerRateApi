﻿using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerRateApi
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Beer> Beers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }
        
    }
}
