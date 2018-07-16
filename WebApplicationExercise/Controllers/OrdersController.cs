using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApplicationExercise.Core;
using WebApplicationExercise.Models;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderService _orderService;
        public OrdersController()
        {
            // or via IoC
            _orderService = new OrderService();
        }
        
        //todo: add async

        [HttpGet]
        public Order GetOrder(Guid orderId)
        {
            return _orderService.GetBy(orderId);
        }

        [HttpPost]
        public void UpdateOrder([FromBody]Order order)
        {
            _orderService.UpdateOrder(order);
        }

        [HttpPut]
        [ExceptionHandler]
        public Order CreateOrder([FromBody]Order order)
        {
            return _orderService.CreateOrder(order);
        }

        [HttpGet]
        public IEnumerable<Order> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            var orders = _orderService.GetAll();

            if (from != null && to != null)
                orders = _orderService.FilterByDate(orders, from.Value, to.Value);

            if (customerName != null)
                orders = _orderService.FilterByCustomer(orders, customerName);

            return _orderService.FilterByCustomer(orders);
        }
    }
}