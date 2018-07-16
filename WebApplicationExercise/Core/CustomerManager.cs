using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            //todo move to settings
            return customerName != "Hidden Joe";
        }
    }
}