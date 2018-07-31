using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationExercise.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public double CreatedDate { get; set; }

        public string CustomerName { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}