using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using System.Security.Claims;

namespace myshop.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM = new ShoppingCartVM()
        {
            CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product")
        };

        foreach (var item in ShoppingCartVM.CartList)
        {
            item.Product.Price = item.Product.Price * item.Quantity;
            ShoppingCartVM.OrderTotal += item.Product.Price;
        }
        return View(ShoppingCartVM);
    }

    public IActionResult Plus(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
        _unitOfWork.ShoppingCart.IncreaseCount(cart, 1);
        _unitOfWork.Complete();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
        if (cart.Quantity == 1)
        {
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index), "Home");
        }
        else
        {
            _unitOfWork.ShoppingCart.DecreaseCount(cart, 1);
        }
        _unitOfWork.Complete();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
        _unitOfWork.ShoppingCart.Remove(cart);
        _unitOfWork.Complete();
        return RedirectToAction(nameof(Index));
    }
}
