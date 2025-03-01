using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;

namespace myshop.DataAccess.Implementation;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Category category)
    {
        var CategoryInDb = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
        if (CategoryInDb != null) 
        {
            CategoryInDb.CategoryName = category.CategoryName;
            CategoryInDb.CategoryDescription = category.CategoryDescription;
        }
    }
}
