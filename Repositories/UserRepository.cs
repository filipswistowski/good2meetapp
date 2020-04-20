using activitiesapp.Models;
using activitiesapp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationContext _applicationContext;

        public UserRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<User[]> GetAllUsersAsync()
        {
            IQueryable<User> usersQuery = _applicationContext.Users;
            return usersQuery.ToArray();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            IQueryable<User> query = _applicationContext.Users;

            query = query.Where(c => c.UserId == id);

            return await query.FirstOrDefaultAsync();
        }

        public void CreateUser(User user)
        {
            _applicationContext.Add(user);
        }
    }
}