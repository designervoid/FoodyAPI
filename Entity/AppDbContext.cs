using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Entity.AppDbContext {
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodType> FoodTypes { get; set; }
        public DbSet<MealItem> MealItems { get; set; }
    }
}