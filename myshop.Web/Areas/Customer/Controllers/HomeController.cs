using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using System.Security.Claims;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeEntity: "Category"),
                Quantity = 1
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserID = claim.Value;

            ShoppingCart userCartObj = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserID == claim.Value && u.ProductId == shoppingCart.ProductId
                );

            if (userCartObj == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(userCartObj, shoppingCart.Quantity);
            }
            _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
    }
}
