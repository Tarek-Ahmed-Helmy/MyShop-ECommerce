using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;

namespace myshop.DataAccess.Implementation;

public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
    private readonly ApplicationDbContext _context;
    public CartItemRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public int DecreaseCount(CartItem cartItem, int count)
    {
        cartItem.Quantity -= count;
        return cartItem.Quantity;
    }

    public int IncreaseCount(CartItem cartItem, int count)
    {
        cartItem.Quantity += count;
        return cartItem.Quantity;
    }
}
