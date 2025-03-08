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

        var cartItems = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product");

        var cartItemsVM = cartItems.Select(item => new CartItemVM
        {
            Id = item.Id,
            Quantity = item.Quantity,
            Price = item.Product.Price,
            ProductName = item.Product.ProductName,
            ImgPath = item.Product.ImgPath
        }).ToList();

        ShoppingCartVM = new ShoppingCartVM()
        {
            CartItems = cartItemsVM,
            TotalAmount = cartItemsVM.Sum(item => item.Price * item.Quantity)
        };
        return View(ShoppingCartVM);
    }

    public IActionResult Plus(int cartId)
    {
        var cart = _unitOfWork.CartItem.GetFirstOrDefault(c => c.Id == cartId);
        _unitOfWork.CartItem.IncreaseCount(cart, 1);
        _unitOfWork.Complete();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cart = _unitOfWork.CartItem.GetFirstOrDefault(c => c.Id == cartId);
        if (cart.Quantity == 1)
        {
            _unitOfWork.CartItem.Remove(cart);
            var count = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count - 1;
            HttpContext.Session.SetInt32(SD.SessionKey, count);
        }
        else
        {
            _unitOfWork.CartItem.DecreaseCount(cart, 1);
        }
        _unitOfWork.Complete();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cart = _unitOfWork.CartItem.GetFirstOrDefault(c => c.Id == cartId);
        _unitOfWork.CartItem.Remove(cart);
        _unitOfWork.Complete();
        var count = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
        HttpContext.Session.SetInt32(SD.SessionKey, count);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var cartItems = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product");

        var cartItemsVM = cartItems.Select(item => new CartItemVM
        {
            Id = item.Id,
            Quantity = item.Quantity,
            Price = item.Product.Price,
            ProductName = item.Product.ProductName,
            ImgPath = item.Product.ImgPath
        }).ToList();

        ShoppingCartVM = new ShoppingCartVM()
        {
            CartItems = cartItemsVM,
            TotalAmount = cartItemsVM.Sum(item => item.Price * item.Quantity)
        };

        var AppUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
        ShoppingCartVM.FullName = AppUser.FullName;
        ShoppingCartVM.PhoneNumber = AppUser.PhoneNumber;
        ShoppingCartVM.Address = AppUser.Address;
        ShoppingCartVM.City = AppUser.City;
        ShoppingCartVM.Email = AppUser.Email;

        return View(ShoppingCartVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout(Order order)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var cartList = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == claim.Value, includeEntity: "Product");

        order.OrderStatus = SD.Pending;
        order.OrderDate = DateTime.Now;
        order.PaymentStatus = SD.Pending;
        order.ApplicationUserId = claim.Value;

        foreach (var item in cartList)
        {
            order.TotalAmount += (item.Product.Price * item.Quantity);
        }

        _unitOfWork.Order.Add(order);
        _unitOfWork.Complete();

        foreach (var item in cartList)
        {
            OrderDetail orderDetail = new OrderDetail()
            {
                ProductId = item.ProductId,
                OrderId = order.Id,
                Price = item.Product.Price,
                Quantity = item.Quantity
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Complete();
        }
        // payment process
        var domain = "https://localhost:5289/";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
                
            Mode = "payment",
            SuccessUrl = domain + $"Customer/Cart/OrderConfirmation/{order.Id}",
            CancelUrl = domain + "Customer/Cart/Index",
        };

        foreach (var item in cartList)
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

        order.SessionId = session.Id;
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

        List<CartItem> cartList = _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == order.ApplicationUserId).ToList();
        HttpContext.Session.Clear();
        _unitOfWork.CartItem.RemoveRange(cartList);
        _unitOfWork.Complete();

        return View(id);
    }
}
