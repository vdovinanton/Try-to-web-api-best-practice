using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationExercise.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }
        public double Price { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}