using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Core
{
    public interface ICustomerService
    {
        bool IsCustomerVisible(string customerName);
    }

    public class CustomerService: ICustomerService
    {
        public bool IsCustomerVisible(string customerName)
        {
            return customerName != Settings.Instance.CustomerName;
        }
    }
}