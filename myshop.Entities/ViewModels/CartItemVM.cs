using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.ViewModels;

public class CartItemVM
{
    public int Id { get; set; }

    [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string ProductName { get; set; }

    public string ImgPath { get; set; }
}
