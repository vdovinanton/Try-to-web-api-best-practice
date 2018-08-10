using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Repository.Models;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Core
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string _url = $"https://free.currencyconverterapi.com";
        private readonly string _currentCurrency = Settings.Instance.CurrenctCurrency;
        

        public async Task<Dictionary<string, double>> GetCurrency(string currency)
        {
            var paramether = $"/api/v6/convert?q={_currentCurrency}_{currency}&compact=ultra";

            using (var client = new HttpClient())
            {
                string responseBody = await client.GetStringAsync(_url + paramether);
                return JsonConvert.DeserializeObject<Dictionary<string, double>>(responseBody);
            }
        }

        public async Task<List<Order>> ConvertOrdersAsync(List<Order> orders, string currency)
        {
            var currencyKey = $"{_currentCurrency}_{currency}";
            var currencyRate = await GetCurrency(currency);

            foreach (var product in orders.SelectMany(_ => _.Products))
                    product.Price *= Math.Round(currencyRate[currencyKey], 3);

            return orders;
        }

        public async Task<Order> ConvertOrderAsync(Order order, string currency)
        {
            var result = await ConvertOrdersAsync(new List<Order> { order }, currency);
            return result.SingleOrDefault();
        }
    }
}