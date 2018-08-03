using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Interfaces;

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
    }
}