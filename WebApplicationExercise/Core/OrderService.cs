using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationExercise.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using WebApplicationExercise.Utils;

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
        /// Filter <see cref="IEnumerable{Order}"/> by time rane or/and by <paramref name="customerName"/>
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        /// <returns></returns>
        Task<IEnumerable<Order>> OrderFilterAsync(DateTime from, DateTime to, string customerName);

        /// <summary>
        /// Modify current <see cref="Order"/>
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one of two dates is empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="to"/> earlier then <paramref name="from"/></exception>
        Task<Guid> UpdateOrCreateOrderAsync(Order order);

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

        private IQueryable<Order> FilterByCustomer()
        {
            Logger.Instance.Information($"Filtered orders by customer {Settings.Instance.CustomerName}");
            return _db.Orders
                .AsNoTracking()
                .Where(o => _customerService.IsCustomerVisible(o.Customer));
        }

        private IQueryable<Order> FilterByCustomer(string customerName)
        {
            Logger.Instance.Information($"Filtered orders by customer {customerName}");
            return _db.Orders
                .AsNoTracking()
                .Where(o => o.Customer == customerName);
        }

        private IQueryable<Order> FilterByDate(DateTime from, DateTime to)
        {
            Logger.Instance.Information($"Filtered orders by date");
            return _db.Orders
                .AsNoTracking()
                .Where(o => o.CreatedDate >= DbFunctions.TruncateTime(from) && o.CreatedDate < DbFunctions.TruncateTime(to));
        }

        public async Task<IEnumerable<Order>> OrderFilterAsync(DateTime from, DateTime to, string customerName)
        {
            if (DateTime.Compare(from, to) >= 0)
                throw new ArgumentOutOfRangeException($"{nameof(to)} date can't be earlier than {nameof(from)}");

            IQueryable<Order> result = FilterByCustomer();
            if (from != null && to != null)
                result = result.Union(FilterByDate(from, to));

            if(customerName != null)
                result = result.Union(FilterByCustomer(customerName));

            return await result.ToListAsync();
        }

        public async Task<Guid> UpdateOrCreateOrderAsync(Order order)
        {
            var originalOrder = _db.Orders
                .AsNoTracking()
                .FirstOrDefault(_ => _.Id == order.Id);

            if (!_db.Orders.AsNoTracking().Any(_ => _.Id == order.Id))
            {
                _db.Orders.Add(order);
                Logger.Instance.Information($"Added orderId - {order.Id} with {order.Products.Count} products");
            }
            else
            {
                _db.Entry(order).State = EntityState.Modified;
                foreach(var product in order.Products)
                    _db.Entry(product).State = EntityState.Modified;

                Logger.Instance.Information($"Modified orderId - {order.Id} with {order.Products.Count} products");
            }

            await _db.SaveChangesAsync();

            return order.Id;
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
    }
}