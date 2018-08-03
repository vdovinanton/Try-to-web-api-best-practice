
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Core.Interfaces
{
    public interface ICustomerService
    {
        bool IsCustomerVisible(string customerName, out string swap);

        bool IsCustomerVisible(string customerName);
    }
}