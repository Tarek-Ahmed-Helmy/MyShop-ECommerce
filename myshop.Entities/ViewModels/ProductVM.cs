using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.ViewModels;

public class ProductVM
{
    public int Id { get; set; }
    public string ProductName { get; set; }

    public string ProductDescription { get; set; }

    [DisplayName("Image")]
    [ValidateNever]
    public string ImgPath { get; set; }
    public decimal Price { get; set; }

    [DisplayName("Category")]
    public int CategoryId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem>? CategoryList { get; set; }
}
