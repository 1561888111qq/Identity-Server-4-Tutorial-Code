using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Restful.Core.Helpers;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Services;

namespace Restful.Infrastructure.Interfaces
{
    public interface IEnhancedRepository<T> : IRepository<T> where T : Entity
    {
        Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, IPropertyMapping propertyMapping);
        Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, IPropertyMapping propertyMapping, Expression<Func<T, bool>> criteria);
        Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, IPropertyMapping propertyMapping, Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
    }
}