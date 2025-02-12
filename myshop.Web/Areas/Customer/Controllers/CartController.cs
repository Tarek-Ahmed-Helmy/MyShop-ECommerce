using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using Stripe.Checkout;
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
            CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product"),
            Order = new()
        };

        foreach (var item in ShoppingCartVM.CartList)
        {
            ShoppingCartVM.OrderTotal += (item.Product.Price * item.Quantity);
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

    [HttpGet]
    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM = new ShoppingCartVM()
        {
            CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product"),
            Order = new ()
        };

        ShoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
        ShoppingCartVM.Order.FullName = ShoppingCartVM.Order.ApplicationUser.FullName;
        ShoppingCartVM.Order.PhoneNumber = ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
        ShoppingCartVM.Order.Address = ShoppingCartVM.Order.ApplicationUser.Address;
        ShoppingCartVM.Order.City = ShoppingCartVM.Order.ApplicationUser.City;

        foreach (var item in ShoppingCartVM.CartList)
        {
            ShoppingCartVM.OrderTotal += (item.Product.Price * item.Quantity);
        }
        return View(ShoppingCartVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public IActionResult PostSummary(ShoppingCartVM shoppingCartVM)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        shoppingCartVM.CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product");

        shoppingCartVM.Order.OrderStatus = SD.Pending;
        shoppingCartVM.Order.OrderDate = DateTime.Now;
        shoppingCartVM.Order.PaymentStatus = SD.Pending;
        shoppingCartVM.Order.ApplicationUserId = claim.Value;

        foreach (var item in shoppingCartVM.CartList)
        {
            shoppingCartVM.OrderTotal += (item.Product.Price * item.Quantity);
        }

        _unitOfWork.Order.Add(shoppingCartVM.Order);
        _unitOfWork.Complete();

        foreach (var item in shoppingCartVM.CartList)
        {
            OrderDetail orderDetail = new OrderDetail()
            {
                ProductId = item.ProductId,
                OrderId = shoppingCartVM.Order.Id,
                Price = item.Product.Price,
                Quantity = item.Quantity
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Complete();
        }
        // payment process
        var domain = "https://localhost:7167/";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
                
            Mode = "payment",
            SuccessUrl = domain + $"Customer/Cart/OrderConfirmation/{shoppingCartVM.Order.Id}",
            CancelUrl = domain + "Customer/Cart/Index",
        };

        foreach (var item in shoppingCartVM.CartList)
        {
            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.ProductName,
                    },
                    UnitAmount = (long)(item.Product.Price * 100),
                },
                Quantity = item.Quantity,
            };
            options.LineItems.Add(sessionLineItem);
        }

        var service = new SessionService();
        Session session = service.Create(options);

        shoppingCartVM.Order.SessionId = session.Id;
        _unitOfWork.Complete();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult OrderConfirmation(int id)
    {
        Order order = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == id);
        var service = new SessionService();
        Session session = service.Get(order.SessionId);
        if (session.PaymentStatus.ToLower() == "paid")
        {
            _unitOfWork.Order.UpdateOrderStatus(id, SD.Approved, SD.Approved);
            order.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.Complete();
        }

        List<ShoppingCart> cartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == order.ApplicationUserId).ToList();
        HttpContext.Session.Clear();
        _unitOfWork.ShoppingCart.RemoveRange(cartList);
        _unitOfWork.Complete();

        return View(id);
    }
}
