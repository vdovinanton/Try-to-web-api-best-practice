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
        
        [HttpPut]
        public async Task<Guid> UpdateOrCreateOrder([FromBody]Order order)
        {
            return await _orderService.UpdateOrCreateOrderAsync(order);
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            IEnumerable<Order> orders;

            if ((from == null && to == null) && customerName == null)
            {
                orders = await _orderService.GetAllAsync();
            }
            else
            {
                var datetimeFrom = from ?? DateTime.MinValue;
                var datetimeTo = to ?? DateTime.MinValue;
                orders = await _orderService.OrderFilterAsync(datetimeFrom, datetimeTo, customerName);
            }

            return orders;
        }

        [HttpDelete]
        public async Task Remove(Guid orderId)
        {
            await _orderService.RemoveAsync(orderId);
        }
    }
}