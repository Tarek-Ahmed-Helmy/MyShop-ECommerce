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

        // add another logic here for the rest of the home page
        public IActionResult Index()
        {
            //categories


            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }

        public IActionResult ShopIndex(int? page)
        {
            var PageNumber = page ?? 1;
            var PageSize = 9;

            var products = _unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            CartItem cartItem = new CartItem()
            {
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeEntity: "Category"),
                Quantity = 1
            };
            return View(cartItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(CartItem cartItemFromReq)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cartItemFromReq.ApplicationUserID = claim.Value;

            CartItem userCartItem = _unitOfWork.CartItem.GetFirstOrDefault(
                u => u.ApplicationUserID == claim.Value && u.ProductId == cartItemFromReq.ProductId
                );

            if (userCartItem == null)
            {
                _unitOfWork.CartItem.Add(cartItemFromReq);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(
                    SD.SessionKey,
                    _unitOfWork.CartItem.GetAll(u => u.ApplicationUserID == cartItemFromReq.ApplicationUserID).ToList().Count
                    );
            }
            else
            {
                _unitOfWork.CartItem.IncreaseCount(userCartItem, cartItemFromReq.Quantity);
                _unitOfWork.Complete();
            }

            return RedirectToAction(nameof(ShopIndex));
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
