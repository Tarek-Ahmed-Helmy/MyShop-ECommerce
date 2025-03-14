﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FullName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
