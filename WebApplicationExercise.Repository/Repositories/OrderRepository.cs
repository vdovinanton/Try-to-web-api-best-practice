using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplicationExercise.Repository.Interfaces;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Repository.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(DataContext context) : base(context)
        {
        }

        public override Task<List<Order>> GetAllAsync()
        {
            return Context.Set<Order>()
                .AsNoTracking()
                .Include(_ => _.Products)
                .ToListAsync();
        }

        public Task<Order> GetWithProductsAsync(int id)
        {
            return Context.Set<Order>()
                .AsNoTracking()
                .Include(_ => _.Products)
                .FirstOrDefaultAsync(_ => _.Id == id);
        }

        public IQueryable<Order> Find(Expression<Func<Order, bool>> predicate, IQueryable<Order> query = null, string sortTag = null)
        {
            IQueryable<Order> result = Context.Set<Order>()
                .AsNoTracking()
                .Include(_ => _.Products)
                .Where(predicate);

            if (query != null)
            {
                result = query
                    .AsNoTracking()
                    .Where(predicate);
            }

            result = SortBy(sortTag, result);

            return result;
        }

        public IQueryable<Order> Paginate(int startFrom, int pageSize, IQueryable<Order> query)
        {
            return query
                .Skip(() => startFrom)
                .Take(() => pageSize);
        }

        public Task<bool> AnyAsync(int id)
        {
            return Context.Set<Order>()
                .AsNoTracking()
                .AnyAsync(_ => _.Id == id);
        }

        public Order Update(Order order)
        {
            Context.Entry(order).State = EntityState.Modified;
            foreach (var product in order.Products)
                Context.Entry(product).State = EntityState.Modified;

            return order;
        }

        private IQueryable<Order> SortBy(string sortTag, IQueryable<Order> query)
        {
            // sort by default if sort wasnt requried
            sortTag = sortTag ?? "created_date";
            sortTag = sortTag.ToLower();
                

            if (sortTag.IndexOf("customer_name") != -1)
                query = query
                    .OrderBy(_ => _.CustomerName);
                else
            if (sortTag.IndexOf("created_date") != -1)
                    query = query
                    .OrderByDescending(_ => _.CreatedDate);
                else
            if (sortTag.IndexOf("order_amount") != -1)
                query = query
                    .OrderByDescending(_ => _.Products.Sum(p => p.Price));

            return query;
        }
    }
}
