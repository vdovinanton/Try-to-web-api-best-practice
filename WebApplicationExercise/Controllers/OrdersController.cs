using System;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplicationExercise.Core;
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/v1/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderService _orderService;
        public OrdersController()
        {
            // or via IoC
            _orderService = new OrderService();
        }
        
        [HttpGet]
        [Route("{orderId}")]
        public async Task<Order> GetOrder(Guid orderId)
        {
            return await _orderService.GetByIdAsync(orderId);
        }

        [HttpPost]
        public async Task UpdateOrder([FromBody]Order order)
        {
            await _orderService.UpdateOrderAsync(order);
        }

        [HttpPut]
        public async Task CreateOrder([FromBody]Order order)
        {
            await _orderService.CreateOrderAsync(order);
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            IEnumerable<Order> orders;

            // wrong
            //if (from != null && to != null)
            //orders = await _orderService.FilterByDateAsync(from.Value, to.Value);

            //if (customerName != null)
            //orders = await _orderService.FilterByCustomerAsync(customerName);

            orders = await _orderService.FilterByDateAsync(from.Value, to.Value);

            if ((from == null && to == null) && customerName == null)
                orders = await _orderService.GetAllAsync();

            
            return await _orderService.FilterByCustomerAsync(); ;
        }

        [HttpDelete]
        public async Task Remove(Guid orderId)
        {
            await _orderService.RemoveAsync(orderId);
        }
    }
}