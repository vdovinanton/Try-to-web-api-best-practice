using WebApplicationExercise.Core.Interfaces;
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

        public bool IsCustomerVisible(string customerName)
        {
            string swap;
            return IsCustomerVisible(customerName, out swap);
        }
    }
}