using System.Data.Entity;
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Core
{
    public class DataContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}