using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.ViewModels;

public class ShoppingCartVM
{
    public IEnumerable<ShoppingCart> CartList { get; set; }
    public decimal OrderTotal { get; set; }
}
