using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Utilities;

namespace myshop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        ViewBag.Orders = _unitOfWork.Order.GetAll().Count();
        var ApprovedOrders = _unitOfWork.Order.GetAll(o => o.OrderStatus == SD.Approved).Count();
        if (ViewBag.Orders != null && ViewBag.Orders > 0)
        {
            ViewBag.ApprovedOrders = Math.Round(((decimal)ApprovedOrders / (decimal)ViewBag.Orders) * 100, 2);
        }
        else
        {
            ViewBag.ApprovedOrders = 0;
        }
        ViewBag.Users = _unitOfWork.ApplicationUser.GetAll().Count();
        ViewBag.Products = _unitOfWork.Product.GetAll().Count();
        return View();
    }
}
