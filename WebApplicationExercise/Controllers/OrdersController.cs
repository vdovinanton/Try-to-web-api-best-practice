using System;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplicationExercise.Models;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.ViewModels;
using AutoMapper;
using System.Linq;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/v1/orders/{currency}")]
    public class OrdersController : ApiController
    {
        private const string CURRENCY = "USD";

        private readonly IOrderService _orderService;
        private readonly ICurrencyService _currencyService;
        public OrdersController(IOrderService orderService, ICurrencyService currencyService)
        {
            _orderService = orderService;
            _currencyService = currencyService;
        }
        
        [HttpGet]
        [Route("{orderId}")]
        // dont know why second paramether is requred
        public async Task<OrderViewModel> GetOrder(int orderId, string currency = null)
        {
            //var order = await _orderService.GetByIdAsync(0);
            //return _mapper.Map<OrderViewModel>(order);
            return null;
        }
        
        [HttpPut]
        public async Task<int> UpdateOrCreateOrder([FromBody]OrderViewModel orderViewModel)
        {
            //var order = _mapper.Map<Order>(orderViewModel);
            //return await _orderService.UpdateOrCreateOrderAsync(order);

            return await _orderService.UpdateOrCreateOrderAsync(orderViewModel);

            //return await Task.Factory.StartNew(() =>
            //{
            //    return 0;
            //});
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> GetOrders(
            int startFrom = 0,
            int pageSize = 0,
            string currency = null,
            DateTime? from = null, 
            DateTime? to = null, 
            string customerName = null, 
            string sortby = null
            )
        {
            var orders = await _orderService.OrderFilterAsync(startFrom, pageSize, currency, from, to, customerName, sortby);

            //if (!string.IsNullOrEmpty(currency))
            //{
            //    var currencyKey = $"USD_{currency}";
            //    var currencyRate = await _currencyService.GetCurrency(currency);

            //    foreach (var product in orders.SelectMany(_ => _.Products))
            //        product.Price *= currencyRate[currencyKey];
            //}

            //return _mapper.Map<IEnumerable<OrderViewModel>>(orders);
            return orders;
        }

        [HttpDelete]
        public async Task Remove(int orderId)
        {
            await _orderService.RemoveAsync(orderId);
        }
    }
}