using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;
using X.PagedList.Extensions;

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
        public IActionResult Index(int? page)
        { 
            var PageNumber = page ?? 1;
            var PageSize = 8;

            var products = _unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
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
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(
                    SD.SessionKey,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == shoppingCart.ApplicationUserID).ToList().Count
                    );
            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(userCartObj, shoppingCart.Quantity);
                _unitOfWork.Complete();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
