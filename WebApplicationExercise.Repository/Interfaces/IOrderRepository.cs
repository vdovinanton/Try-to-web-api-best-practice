using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Repository.Interfaces
{
    /// <summary>
    /// Represents <see cref="Order"/> repository
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetWithProductsAsync(int id);

        Task<bool> AnyAsync(int id);

        Order Update(Order order);

        /// <summary>
        /// Create or Update <see cref="IQueryable"/> query
        /// </summary>
        IQueryable<Order> Find(Expression<Func<Order, bool>> predicate, IQueryable<Order> query = null, string sortTags = null);

        /// <summary>
        /// Update <see cref="IQueryable"/> query and will add sort before paginate
        /// </summary>
        /// <param name="startFrom">Start from index, started from created date</param>
        /// <param name="pageSize">Amount of items range</param>
        /// <param name="sortTags">By default - <see cref="Order.CreatedDate"/></param>
        /// <returns></returns>
        IQueryable<Order> Paginate(int startFrom, int pageSize, IQueryable<Order> query);
    }
}
