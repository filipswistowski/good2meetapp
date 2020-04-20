using activitiesapp.Models;
using activitiesapp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationContext _applicationContext;

        public CategoryRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Category[]> GetAllCategoriesAsync()
        {
            IQueryable<Category> categoriesQuery = _applicationContext.Category;
            return categoriesQuery.ToArray();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            IQueryable<Category> query = _applicationContext.Category;

            query = query.Where(c => c.CategoryId == id);

            return await query.FirstOrDefaultAsync();
        }

        public void CreateCategory(Category category)
        {
            _applicationContext.Add(category);
        }
    }
}