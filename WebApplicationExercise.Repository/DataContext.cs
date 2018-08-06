using System.Data.Entity;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Repository
{
    public class DataContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}