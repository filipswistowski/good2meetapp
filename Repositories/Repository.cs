using activitiesapp.Models;
using activitiesapp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _appContext;

        public Repository(ApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public IQueryable<T> Get()
        {
            return _appContext.Set<T>().AsNoTracking();
        }

        public void Create(T entity)
        {
            _appContext.Add(entity);
        }

        public void Update(T entity)
        {
            _appContext.Update(entity);
        }

        public void Delete(T entity)
        {
            _appContext.Remove(entity);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return (await _appContext.SaveChangesAsync()) > 0;
        }
    }
}