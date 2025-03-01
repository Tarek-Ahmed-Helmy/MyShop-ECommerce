using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; }

    [Display(Name = "Category Description")]
    public string CategoryDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}
