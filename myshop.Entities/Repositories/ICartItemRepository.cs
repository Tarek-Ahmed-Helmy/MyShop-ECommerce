using myshop.Entities.Models;

namespace myshop.Entities.Repositories;

public interface ICartItemRepository : IGenericRepository<CartItem>
{
    int IncreaseCount(CartItem cartItem, int count);
    int DecreaseCount(CartItem cartItem, int count);
}
