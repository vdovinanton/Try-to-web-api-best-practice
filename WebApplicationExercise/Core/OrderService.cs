using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationExercise.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Interfaces;
using NLog;

namespace WebApplicationExercise.Core
{
    public class OrderService: IOrderService
    {
        private readonly DataContext _db;
        private readonly ICustomerService _customerService;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderService(DataContext db, ICustomerService customerService)
        {
            _db = db;
            _customerService = customerService;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var result = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .ToListAsync();
            _logger.Info($"Get all orders [{result.Count()}]");
            return result;
        }

        public async Task<Order> GetByIdAsync(int orderId)
        {
            _logger.Info($"Get order by Id - {orderId}");
            return await _db.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        private IQueryable<Order> FilterByCustomer(string customerName)
        {
            _logger.Info($"Filtered orders by customer {customerName}");
            return _db.Orders
                .AsNoTracking()
                .Where(o => o.CustomerName == customerName);
        }

        public async Task<IEnumerable<Order>> OrderFilterAsync(
            int startFrom,
            int pageSize,
            string currency,
            DateTime? from,
            DateTime? to,
            string customerName, 
            string sortby
            )
        {
            if ((from != null && to != null) && (DateTime.Compare(from.Value, to.Value) > 0))
                throw new ArgumentOutOfRangeException($"{nameof(to)} date can't be earlier than {nameof(from)}");

            string bannedCustomerName = string.Empty;
            _customerService.IsCustomerVisible(customerName, out bannedCustomerName);

            IQueryable<Order> query = _db.Orders
                .Include(_ => _.Products)
                .Where(_ => _.CustomerName != bannedCustomerName);

            query = SortBy(sortby, query);

            if ((from != null && to != null))
            {
                _logger.Info($"Filtered orders by date");
                query = query.Where(res => res.CreatedDate >= DbFunctions.TruncateTime(from)
                    && res.CreatedDate < DbFunctions.TruncateTime(to));
            }
            if (customerName != null)
            {
                _logger.Info($"Filtered orders by customer {customerName}");
                query = query.Where(res => res.CustomerName == customerName);
            }

            if (startFrom >= 0 && pageSize > 0)
                query = query
                    .Skip(() => startFrom)
                    .Take(() => pageSize);

            _logger.Info($"Taken orders {pageSize} started from {startFrom}");
            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<Order> SortBy(string sort, IQueryable<Order> query)
        {
            sort = sort.ToLower();

            // sort by default if sort wasnt requried
            if (string.IsNullOrEmpty(sort))
                query = query
                    .OrderBy(_ => _.CustomerName);


            if (sort.IndexOf("customer_name") != -1)
                query = query
                    .OrderBy(_ => _.CustomerName);
            else
            if (sort.IndexOf("order_date") != -1)
                query = query
                    .OrderByDescending(_ => _.CreatedDate);
            else
            if (sort.IndexOf("order_amount") != -1)
                query = query
                    .OrderByDescending(_ => _.Products.Sum(p => p.Price));

            _logger.Info($"Orders filtered by {sort}");
            return query;
        }

        public async Task<int> UpdateOrCreateOrderAsync(Order order)
        {
            var originalOrder = _db.Orders
                .AsNoTracking()
                .FirstOrDefault(_ => _.Id == order.Id);

            if (!_db.Orders.AsNoTracking().Any(_ => _.Id == order.Id))
            {
                _db.Orders.Add(order);
                _logger.Info($"Added orderId - {order.Id} with {order.Products.Count} products");
            }
            else
            {
                _db.Entry(order).State = EntityState.Modified;
                foreach(var product in order.Products)
                    _db.Entry(product).State = EntityState.Modified;

                _logger.Info($"Modified orderId - {order.Id} with {order.Products.Count} products");
            }

            await _db.SaveChangesAsync();

            return order.Id;
        }


        public async Task RemoveAsync(int orderId)
        {
            if (!Equals(orderId, default(int)))
            {
                var order = GetByIdAsync(orderId);
                _db.Entry(order).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                _logger.Info($"Remove order Id - {order.Id}");
            }
            else
            {
                throw new ArgumentNullException($"{nameof(orderId)} can't be empty");
            }
        }
    }
}