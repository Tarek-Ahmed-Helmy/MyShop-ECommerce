using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;

namespace myshop.DataAccess.Implementation;

public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;
    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public int DecreaseCount(ShoppingCart shoppingCart, int count)
    {
        shoppingCart.Quantity -= count;
        return shoppingCart.Quantity;
    }

    public int IncreaseCount(ShoppingCart shoppingCart, int count)
    {
        shoppingCart.Quantity += count;
        return shoppingCart.Quantity;
    }
}
