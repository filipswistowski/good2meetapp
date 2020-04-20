using activitiesapp.Models;
using System.Threading.Tasks;

namespace activitiesapp.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User[]> GetAllUsersAsync();

        Task<User> GetUserByIdAsync(int id);

        void CreateUser(User user);

        Task<bool> SaveChangeAsync();
    }
}