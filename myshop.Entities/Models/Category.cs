using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string CategoryName { get; set; }
    public string CategoryDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}
