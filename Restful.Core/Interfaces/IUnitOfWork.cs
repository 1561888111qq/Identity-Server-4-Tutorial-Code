using System.Threading.Tasks;

namespace Restful.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync();
    }
}
