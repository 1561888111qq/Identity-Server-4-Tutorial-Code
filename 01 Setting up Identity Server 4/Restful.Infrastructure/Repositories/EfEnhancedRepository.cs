using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restful.Core.Helpers;
using Restful.Core.Entities;
using Restful.Infrastructure.Database;
using Restful.Infrastructure.Extensions;
using Restful.Infrastructure.Interfaces;
using Restful.Infrastructure.Services;

namespace Restful.Infrastructure.Repositories
{
    public class EfEnhancedRepository<T> : EfRepository<T>, IEnhancedRepository<T> where T : Entity
    {
        public EfEnhancedRepository(MyContext context): base(context)
        {
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, IPropertyMapping propertyMapping)
        {
            var collectionBeforePaging = Context.Set<T>().ApplySort(parameters.OrderBy, propertyMapping);
            var count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<T>(parameters.PageIndex, parameters.PageSize, count, items);
            return result;
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, IPropertyMapping propertyMapping, Expression<Func<T, bool>> criteria) 
        {
            var collectionBeforePaging = Context.Set<T>().Where(criteria).ApplySort(parameters.OrderBy, propertyMapping);
            var count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<T>(parameters.PageIndex, parameters.PageSize, count, items);
            return result;
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, IPropertyMapping propertyMapping, Expression<Func<T, bool>> criteria, 
            params Expression<Func<T, object>>[] includes)
        {
            var collectionBeforePaging = includes
                .Aggregate(Context.Set<T>().Where(criteria).ApplySort(parameters.OrderBy, propertyMapping),
                    (current, include) => current.Include(include));
            var count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<T>(parameters.PageIndex, parameters.PageSize, count, items);
            return result;
        }
    }
}
