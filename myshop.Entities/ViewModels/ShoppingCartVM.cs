using myshop.Entities.Models;

namespace myshop.Entities.ViewModels;

public class ShoppingCartVM
{

    public decimal TotalAmount { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public IEnumerable<CartItemVM> CartItems { get; set; }
}
