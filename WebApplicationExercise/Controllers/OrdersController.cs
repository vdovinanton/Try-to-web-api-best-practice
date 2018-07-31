using System;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplicationExercise.Models;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.ViewModels;
using WebApplicationExercise.Utils;
using System.Linq;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/v1/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        [HttpGet]
        [Route("{orderId}")]
        public async Task<OrderViewModel> GetOrder(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate.ConvertFromUnixToStringUtc(),
                CustomerName = order.CustomerName,
                Products = order.Products?.Select(_ => new ProductViewModel {
                    Id = _.Id,
                    Name = _.Name,
                    Price = _.Price
                }).ToList()
            };

            return orderViewModel;
        }
        
        [HttpPut]
        public async Task<int> UpdateOrCreateOrder([FromBody]OrderViewModel orderViewModel)
        {
            var order = new Order
            {
                Id = orderViewModel.Id,
                CreatedDate = orderViewModel.CreatedDate.ConvertFromStringToUnix(),
                CustomerName = orderViewModel.CustomerName,
                Products = orderViewModel.Products?.Select(_ => new Product
                {
                    Id = _.Id,
                    Name = _.Name,
                    OrderId = orderViewModel.Id,
                    Price = _.Price
                }).ToList()
            };

            return await _orderService.UpdateOrCreateOrderAsync(order);
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
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

            var ordersViewModel = orders.Select(_ => new OrderViewModel
            {
                Id = _.Id,
                CreatedDate = _.CreatedDate.ConvertFromUnixToStringUtc(),
                CustomerName = _.CustomerName,
                Products = _.Products?.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList()
            }).ToList();
            
            return ordersViewModel;
        }

        [HttpDelete]
        public async Task Remove(int orderId)
        {
            await _orderService.RemoveAsync(orderId);
        }
    }
}