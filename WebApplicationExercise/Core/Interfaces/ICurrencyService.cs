using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Core.Interfaces
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, double>> GetCurrency(string currency);
        Task<List<Order>> ConvertOrdersAsync(List<Order> orders, string currency);
        Task<Order> ConvertOrderAsync(Order order, string currency)
    }
}