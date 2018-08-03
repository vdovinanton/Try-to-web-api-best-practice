using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Core.Interfaces
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
        /// Get by <see cref="int"/> Id
        /// </summary>
        /// <param name="orderId">Specific Id</param>
        Task<Order> GetByIdAsync(int orderId);

        /// <summary>
        /// Filter <see cref="IEnumerable{Order}"/> by time rane or/and by <paramref name="customerName"/>
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        /// <returns></returns>
        Task<IEnumerable<Order>> OrderFilterAsync(int skip, int take, string currency, DateTime? from, DateTime? to, string customerName, string sortby);

        /// <summary>
        /// Modify current <see cref="Order"/>
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one of two dates is empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="to"/> earlier then <paramref name="from"/></exception>
        Task<int> UpdateOrCreateOrderAsync(Order order);

        /// <exception cref="ArgumentNullException">Thrown when <paramref name="order"/> Id is null</exception>
        Task RemoveAsync(int orderId);
    }
    #endregion
}