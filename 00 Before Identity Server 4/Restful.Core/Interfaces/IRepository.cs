using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Restful.Core.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        Task<T> GetSingleAsync(Expression<Func<T, bool>> criteria);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        IEnumerable<T> ListAll();
        Task<List<T>> ListAllAsync();

        IEnumerable<T> List(Expression<Func<T, bool>> criteria);
        Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria);
        IEnumerable<T> List(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
        Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);

        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> criteria);
        void AddRange(IEnumerable<T> entities);
        void DeleteRange(IEnumerable<T> entities);
        void Attach(T entity);
        void AttachRange(IEnumerable<T> entities);
        void Detach(T entity);
        void DetachRange(IEnumerable<T> entities);
        void AttachAsModified(T entity);
    }
}
