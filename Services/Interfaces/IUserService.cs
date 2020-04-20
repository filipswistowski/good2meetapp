using activitiesapp.Helpers;
using activitiesapp.Models;
using System.Threading.Tasks;

namespace activitiesapp.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string email, string password);

        string HashPassword(string value, string salt);

        bool Validate(string value, string salt, string hash);

        string CreateSalt();
    }
}