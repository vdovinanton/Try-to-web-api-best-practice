using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationExercise.Repository.Interfaces;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Threading.Tasks;
using WebApplicationExercise.Repository.Models;

namespace WebApplicationExercise.Repository.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DataContext Context;
        public Repository(DataContext context)
        {
            Context = context;
        }

        public virtual Task<TEntity> GetAsync(int id)
        {
            return Context.Set<TEntity>().FindAsync(id);
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return Context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().AsNoTracking().Where(predicate);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entitys)
        {
            Context.Set<TEntity>().AddRange(entitys);
        }

        public void Remove(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            //Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entitys)
        {
            Context.Set<TEntity>().RemoveRange(entitys);
        }

        public Task<int> CompleteAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
