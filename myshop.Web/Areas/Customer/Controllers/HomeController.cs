using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;

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

        public IActionResult Details(int id) 
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeEntity: "Category"),
                Quantity = 1
            };
            return View(shoppingCart);
        }
    }
}
