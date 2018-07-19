using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationExercise.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WebApplicationExercise.Core
{
    #region interface
    /// <summary>
    /// Represents OrderService
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Get all <see cref="Order"/> with <see cref="Product"/>
        /// </summary>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Get by <see cref="Guid"/> Id
        /// </summary>
        /// <param name="orderId">Specific Id</param>
        Task<Order> GetByIdAsync(Guid orderId);

        /// <summary>
        /// Filtering by current customer name
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="customerName"/> is null</exception>
        Task<IEnumerable<Order>> FilterByCustomerAsync(string customerName);

        /// <summary>
        /// Filtering by default customer name
        /// </summary>
        Task<IEnumerable<Order>> FilterByCustomerAsync();

        /// <summary>
        /// Filtering by range of <see cref="DateTime"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="to"/> earlier than <paramref name="from"/></exception>
        Task<IEnumerable<Order>> FilterByDateAsync(DateTime from, DateTime to);
        
        /// <param name="order">New <see cref="Order"/></param>
        /// <returns><see cref="Order"/> with new <see cref="Guid"/> Id</returns>
        Task<Order> CreateOrderAsync(Order order);

        /// <summary>
        /// Modify current <see cref="Order"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        Task UpdateOrderAsync(Order order);

        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        Task RemoveAsync(Guid orderId);
    }
    #endregion
    public class OrderService: IOrderService
    {
        private readonly MainDataContext _db;
        private readonly ICustomerService _customerService;

        public OrderService()
        {
            // todo via IoC
            _db = new MainDataContext();
            _customerService = new CustomerService();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var result = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .ToListAsync();
            Logger.Instance.Information($"Get all orders [{result.Count()}]");
            return result;
        }

        public async Task<Order> GetByIdAsync(Guid orderId)
        {
            Logger.Instance.Information($"Get order by Id - {orderId}");
            return await _db.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> FilterByCustomerAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentNullException(nameof(customerName) + "can't be empty or null");

            var result = await _db.Orders
                .AsNoTracking()
                .Where(o => o.Customer == customerName)
                .ToListAsync();
            Logger.Instance.Information($"Filtered orders by {customerName} customer [{result.Count}]");
            return result;
        }

        public async Task<IEnumerable<Order>> FilterByCustomerAsync()
        {
            var result = await _db.Orders
                .AsNoTracking()
                .Where(o => _customerService.IsCustomerVisible(o.Customer))
                .ToListAsync();
            Logger.Instance.Information($"Filtered orders by default customer [{result.Count()}]");
            return result;
        }

        public async Task<IEnumerable<Order>> FilterByDateAsync(DateTime from, DateTime to)
        {
            if (DateTime.Compare(from, to) >= 0)
                throw new ArgumentOutOfRangeException($"{nameof(to)} date can't be earlier than {nameof(from)}");

            if (from == null && to == null)
                return null;

            var result = await _db.Orders
                .AsNoTracking()
                .Where(o => o.CreatedDate >= DbFunctions.TruncateTime(from) && o.CreatedDate < DbFunctions.TruncateTime(to))
                .ToListAsync();
            Logger.Instance.Information($"Filtered orders by date [{result.Count()}]");
            return result;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (Equals(order.Id, Guid.Empty))
                throw new ArgumentNullException($"{nameof(order.Id)} can't be empty");

            await UpdateOrCreateAsync(order);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await UpdateOrCreateAsync(order);
        }

        public async Task RemoveAsync(Guid orderId)
        {
            if (!Equals(orderId, Guid.Empty))
            {
                var order = GetByIdAsync(orderId);
                _db.Entry(order).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                Logger.Instance.Information($"Remove order Id - {order.Id}");
            }
            else
            {
                throw new ArgumentNullException($"{nameof(orderId)} can't be empty");
            }
        }

        private async Task<Order> UpdateOrCreateAsync(Order order)
        {
            if (Equals(order.Id, Guid.Empty))
            {
                _db.Orders.Add(order);
                Logger.Instance.Information($"Added orderId - {order.Id} with {order.Products.Count} products");
            }
            else
            {
                _db.Entry(order).State = EntityState.Modified;

                foreach (var product in order.Products)
                    _db.Entry(product).State = EntityState.Modified;

                Logger.Instance.Information($"Modified orderId - {order.Id} with {order.Products.Count} products");
            }

            await _db.SaveChangesAsync();
            return order;
        }
    }
}