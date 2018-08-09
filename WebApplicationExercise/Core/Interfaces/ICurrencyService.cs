using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Core.Interfaces
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, double>> GetCurrency(string currency);

        Task<List<Order>> ConvertAsync(List<Order> orders, string currency);
    }
}