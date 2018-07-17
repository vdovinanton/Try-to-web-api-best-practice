using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
        public HttpResponseMessage GetOrder(Guid orderId)
        {
            var result = _orderService.GetBy(orderId);
            var statusCode = result != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return Request.CreateResponse(statusCode, result);
        }

        [HttpPost]
        [Route]
        public HttpResponseMessage UpdateOrder([FromBody]Order order)
        {
            _orderService.UpdateOrder(order);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        [Route]
        public HttpResponseMessage CreateOrder([FromBody]Order order)
        {
            return Request.CreateResponse(HttpStatusCode.Created, _orderService.CreateOrder(order));
        }

        [HttpGet]
        [Route]
        public HttpResponseMessage GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {   
            var orders = _orderService.GetAll();

            if (from != null && to != null)
                orders = _orderService.FilterByDate(orders, from.Value, to.Value);

            if (customerName != null)
                orders = _orderService.FilterByCustomer(orders, customerName);

            // todo: maybe move to attribute filter
            var filteredByCustomer = _orderService.FilterByCustomer(orders);
            return Request.CreateResponse(HttpStatusCode.OK, filteredByCustomer);
        }

        [HttpDelete]
        [Route]
        public HttpResponseMessage Remove(Guid orderId)
        {
            _orderService.Remove(orderId);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}