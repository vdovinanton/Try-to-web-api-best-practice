using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationExercise.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public double Price { get; set; }
    }
}