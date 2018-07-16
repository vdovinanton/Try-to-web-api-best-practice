using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Core
{
    public interface ICustomerService
    {
        bool IsCustomerVisible(string customerName);
    }

    //move to attribute
    public class CustomerService: ICustomerService
    {
        public bool IsCustomerVisible(string customerName)
        {
            return customerName != Settings.Instance.CustomerName;
        }
    }
}