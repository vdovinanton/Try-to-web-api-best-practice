using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Core
{
    public class CurrencyService : ICurrencyService
    {
        public async Task<Dictionary<string, double>> GetCurrency(string currency)
        {
            var uri = $"https://free.currencyconverterapi.com/api/v6/convert?q=USD_{currency}&compact=ultra";

            using (var client = new HttpClient())
            {
                string responseBody = await client.GetStringAsync(uri);
                return JsonConvert.DeserializeObject<Dictionary<string, double>>(responseBody);
            }
        }

        public async Task<List<Order>> ConvertAsync(List<Order> orders, string currency)
        {
            var currencyKey = $"USD_{currency}";
            var currencyRate = await GetCurrency(currency);

            foreach (var product in orders.SelectMany(_ => _.Products))
                product.Price *= Math.Round(currencyRate[currencyKey], 3);

            return orders;
        }
    }
}