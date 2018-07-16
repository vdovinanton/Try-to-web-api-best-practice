using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationExercise.Models;
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
        IEnumerable<Order> GetAll();

        /// <summary>
        /// Get by <see cref="Guid"/> Id
        /// </summary>
        /// <param name="orderId">Specific Id</param>
        Order GetBy(Guid orderId);

        /// <summary>
        /// Filtering by current customer name
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="customerName"/> is null</exception>
        IEnumerable<Order> FilterByCustomer(IEnumerable<Order> orders, string customerName);

        /// <summary>
        /// Filtering by default customer name
        /// </summary>
        IEnumerable<Order> FilterByCustomer(IEnumerable<Order> orders);

        /// <summary>
        /// Filtering by range of <see cref="DateTime"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="to"/> earlier than <paramref name="from"/></exception>
        IEnumerable<Order> FilterByDate(IEnumerable<Order> orders, DateTime from, DateTime to);

        /// <param name="order">New <see cref="Order"/></param>
        /// <returns><see cref="Order"/> with new <see cref="Guid"/> Id</returns>
        Order CreateOrder(Order order);

        /// <summary>
        /// Modify current <see cref="Order"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        void UpdateOrder(Order order);

        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        void Remove(Order order);
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

        public IEnumerable<Order> GetAll()
        {
            var result = _db.Orders.Include(o => o.Products);
            Logger.Instance.Information($"Get all orders [{result.Count()}]");
            return result;
        }

        public Order GetBy(Guid orderId)
        {
            Logger.Instance.Information($"Get order by Id - {orderId}");
            return _db.Orders.Include(o => o.Products).Single(o => o.Id == orderId);
        }

        public IEnumerable<Order> FilterByCustomer(IEnumerable<Order> orders, string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentNullException(nameof(customerName) + "can't be empty or null");

            var result = orders.Where(o => o.Customer == customerName);
            Logger.Instance.Information($"Filtered orders by {customerName} customer [{orders.Count()}/{result.Count()}]");
            return result;
        }

        public IEnumerable<Order> FilterByCustomer(IEnumerable<Order> orders)
        {
            var result = orders.Where(o => _customerService.IsCustomerVisible(o.Customer));
            Logger.Instance.Information($"Filtered orders by default customer [{orders.Count()}/{result.Count()}]");
            return result;
        }

        public IEnumerable<Order> FilterByDate(IEnumerable<Order> orders, DateTime from, DateTime to)
        {
            if (DateTime.Compare(from, to) >= 0)
                throw new ArgumentOutOfRangeException($"{nameof(to)} date can't be earlier than {nameof(from)}");

            var result = orders.Where(o => o.CreatedDate >= from && o.CreatedDate < to);
            Logger.Instance.Information($"Filtered orders by date [{orders.Count()}/{result.Count()}]");
            return result;
        }

        public void UpdateOrder(Order order)
        {
            if (Equals(order.Id, Guid.Empty))
                throw new ArgumentNullException($"{nameof(order.Id)} can't be empty");

            UpdateOrCreate(order);
        }

        public Order CreateOrder(Order order)
        {
            return UpdateOrCreate(order);
        }

        public void Remove(Order order)
        {
            if (!Equals(order.Id, Guid.Empty))
            {
                _db.Entry(order).State = EntityState.Deleted;
                _db.SaveChanges();
                Logger.Instance.Information($"Remove order Id - {order.Id}");
            }
            else
            {
                throw new ArgumentNullException($"{nameof(order.Id)} can't be empty");
            }
        }

        private Order UpdateOrCreate(Order order)
        {
            var entityState = EntityState.Unchanged;
            using (_db)
            {
                entityState = _db.Orders.Any(_ => _.Id == order.Id) ?
                    EntityState.Modified :
                    EntityState.Added;
                
                _db.Entry(order).State = entityState;
                _db.SaveChanges();
            }

            var logTemplate = entityState == EntityState.Added ?
                    $"Added" :
                    $"Modified ";

            Logger.Instance.Information(logTemplate + $"orderId - {order.Id}");

            return order;
        }
    }
}