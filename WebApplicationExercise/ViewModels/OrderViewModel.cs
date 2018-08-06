using System.Collections.Generic;

namespace WebApplicationExercise.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string CreatedDate { get; set; }
        public string Customer { get; set; }
        public ICollection<ProductViewModel> Products { get; set; }
    }
}