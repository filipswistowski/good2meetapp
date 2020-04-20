using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<bool> SaveChangeAsync();
    }
}