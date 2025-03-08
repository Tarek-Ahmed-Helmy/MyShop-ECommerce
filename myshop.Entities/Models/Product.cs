using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string ProductName { get; set; }

    [Required]
    public string ProductDescription { get; set; }

    [DisplayName("Image")]
    [ValidateNever]
    public string ImgPath { get; set; }
    [Required]
    public decimal Price { get; set; }

    [Required]
    [DisplayName("Category")]
    public int CategoryId { get; set; }
    [ValidateNever]
    public Category Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
