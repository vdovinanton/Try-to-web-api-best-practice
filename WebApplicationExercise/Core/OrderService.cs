using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Interfaces;
using NLog;
using WebApplicationExercise.Repository.Interfaces;
using WebApplicationExercise.ViewModels;
using AutoMapper;
using WebApplicationExercise.Repository.Models;
using System.Data.Entity;

namespace WebApplicationExercise.Core
{
    public class OrderService: IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repository;
        private readonly ICustomerService _customerService;
        private readonly ICurrencyService _currencyService;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderService(ICustomerService customerService, IOrderRepository orderRepository, ICurrencyService currencyService, IMapper mapper)
        {
            _mapper = mapper;
            _repository = orderRepository;
            _customerService = customerService;
            _currencyService = currencyService;
        }

        public async Task<List<OrderViewModel>> GetAllAsync()
        {
            var result = await _repository
                .GetAllAsync();
            _logger.Info($"Get all orders [{result.Count}]");
            return _mapper.Map<List<OrderViewModel>>(result);
        }

        public async Task<OrderViewModel> GetByIdAsync(int orderId, string currency = null)
        {
            _logger.Info($"Get order by Id - {orderId}");
            var order = await _repository.GetWithProductsAsync(orderId);

            if(!string.IsNullOrEmpty(currency))
                order = await _currencyService.ConvertOrderAsync(order, currency);

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<IEnumerable<OrderViewModel>> OrderFilterAsync(
            int startFrom,
            int pageSize,
            string currency,
            DateTime? from,
            DateTime? to,
            string customerName,
            string sortby
            )
        {

            string bannedCustomerName;
            _customerService.IsCustomerVisible(customerName, out bannedCustomerName);

            // create query
            var query = _repository
                .Find(_ => _.CustomerName != bannedCustomerName, null, sortby);

            // filtering
            if ((from != null && to != null))
            {
                _logger.Info($"Filtered orders by date");
                query = _repository.Find(_ => _.CreatedDate >= DbFunctions.TruncateTime(from) 
                    && _.CreatedDate < DbFunctions.TruncateTime(to), query);
            }
            if (customerName != null)
            {
                _logger.Info($"Filtered orders by customer {customerName}");
                query = _repository.Find(_ => _.CustomerName == customerName, query);
            }

            // paginate
            if (startFrom >= 0 && pageSize >= 1)
            {
                query = _repository
                    .Paginate(startFrom, pageSize, query);
                _logger.Info($"Paginated orders {pageSize} started from {startFrom}");
            }

            var result = await query
                .AsNoTracking()
                .ToListAsync();

            if(!string.IsNullOrEmpty(currency))
                result = await _currencyService.ConvertOrdersAsync(result, currency);

            _logger.Info($"Total taken orders {result.Count}");

            return _mapper.Map<List<OrderViewModel>>(result);
        }


        public async Task<int> UpdateOrCreateOrderAsync(OrderViewModel order)
        {
            var mappedOrder = _mapper.Map<Order>(order);

            if (!await _repository.AnyAsync(order.Id))
            {
                _repository.Add(mappedOrder);
                _logger.Info($"Added order with {order.Products.Count} products");
            }
            else
            {
                _repository.Update(mappedOrder);
                _logger.Info($"Modified orderId - {order.Id} with {order.Products.Count} products");
            }

            await _repository.CompleteAsync();

            return mappedOrder.Id;
        }

        public async Task RemoveAsync(int orderId)
        {
            var order = await _repository.GetAsync(orderId);
            _repository.Remove(order);
            await _repository.CompleteAsync();
        }
    }
}