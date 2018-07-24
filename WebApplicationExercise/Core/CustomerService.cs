using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Core
{
    public class CustomerService: ICustomerService
    {
        public bool IsCustomerVisible(string customerName)
        {
            return customerName != Settings.Instance.CustomerName;
        }
    }
}