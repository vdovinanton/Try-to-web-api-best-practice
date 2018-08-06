using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationExercise.Repository.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CustomerName { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}