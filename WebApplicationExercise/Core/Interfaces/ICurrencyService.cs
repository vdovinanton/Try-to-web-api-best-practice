using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplicationExercise.Core.Interfaces
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, double>> GetCurrency(string currency);
    }
}