using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace myshop.Entities.Models;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    [ValidateNever]
    public Order? Order { get; set; }

    [ValidateNever]
    public Product? Product { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
