using myshop.Entities.Models;

namespace myshop.Entities.ViewModels;

public class ShoppingCartVM
{
    public IEnumerable<ShoppingCart> CartList { get; set; }
    public Order Order { get; set; }
}
