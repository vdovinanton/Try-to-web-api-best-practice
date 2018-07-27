using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationExercise.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        [MaxLength(30)]
        public string Customer { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}