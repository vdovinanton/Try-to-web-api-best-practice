using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationExercise.Repository.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        public double Price { get; set; }

        public Order Order { get; set; }
    }
}