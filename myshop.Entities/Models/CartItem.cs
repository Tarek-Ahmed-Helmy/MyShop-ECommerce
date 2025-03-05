using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.Models;

public class CartItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Quantity { get; set; }
    public string ApplicationUserID { get; set; }

    [ValidateNever]
    public Product? Product { get; set; }

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
