using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Database;

namespace Restful.Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly MyContext Context;

        public EfRepository(MyContext context)
        {
            Context = context;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            var queryableResultWithIncludes = includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));
            return await queryableResultWithIncludes.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> criteria)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(criteria);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            var queryableResultWithIncludes = includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));
            return await queryableResultWithIncludes.FirstOrDefaultAsync(criteria);
        }

        public IEnumerable<T> ListAll()
        {
            return Context.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public IEnumerable<T> List(Expression<Func<T, bool>> criteria)
        {
            return Context.Set<T>().Where(criteria).AsEnumerable();
        }

        public async Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria)
        {
            return await Context.Set<T>().Where(criteria).ToListAsync();
        }

        public IEnumerable<T> List(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            var queryableResultWithIncludes = includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));
            return queryableResultWithIncludes.Where(criteria).AsEnumerable();
        }
        public async Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            var queryableResultWithIncludes = includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));
            return await queryableResultWithIncludes.Where(criteria).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await Context.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await Context.Set<T>().CountAsync(criteria);
        }

        public T Add(T entity)
        {
            Context.Set<T>().Add(entity);
            return entity;
        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void DeleteWhere(Expression<Func<T, bool>> criteria)
        {
            IEnumerable<T> entities = Context.Set<T>().Where(criteria);
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Deleted;
            }
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Context.Set<T>().Remove(entity);
            }
        }

        public void Attach(T entity)
        {
            Context.Set<T>().Attach(entity);
        }

        public void AttachRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Attach(entity);
            }
        }

        public void Detach(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public void DetachRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Detach(entity);
            }
        }

        public void AttachAsModified(T entity)
        {
            Attach(entity);
            Update(entity);
        }
    }
}
