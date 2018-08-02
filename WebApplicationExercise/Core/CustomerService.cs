using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Models;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Core
{
    public class CustomerService: ICustomerService
    {
        public bool IsCustomerVisible(string customerName, out string swap)
        {
            swap = Settings.Instance.CustomerName;
            return customerName != swap;
        }
    }
}