using System;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplicationExercise.Models;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.ViewModels;
using AutoMapper;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/v1/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("{orderId}")]
        public async Task<OrderViewModel> GetOrder(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            return _mapper.Map<OrderViewModel>(order); ;
        }
        
        [HttpPut]
        public async Task<int> UpdateOrCreateOrder([FromBody]OrderViewModel orderViewModel)
        {
            var order = _mapper.Map<Order>(orderViewModel);
            return await _orderService.UpdateOrCreateOrderAsync(order);
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            //IEnumerable<Order> orders;

            //if ((from == null && to == null) && customerName == null)
            //    orders = await _orderService.GetAllAsync();
            //else
            //    orders = await _orderService.OrderFilterAsync(from, to, customerName);

            var orders = await _orderService.OrderFilterAsync(from, to, customerName);

            return _mapper.Map<IEnumerable<OrderViewModel>>(orders);
        }

        [HttpDelete]
        public async Task Remove(int orderId)
        {
            await _orderService.RemoveAsync(orderId);
        }
    }
}