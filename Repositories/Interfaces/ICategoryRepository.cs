using activitiesapp.Models;
using System.Threading.Tasks;

namespace activitiesapp.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category[]> GetAllCategoriesAsync();

        Task<Category> GetCategoryByIdAsync(int id);

        void CreateCategory(Category category);

        Task<bool> SaveChangeAsync();
    }
}