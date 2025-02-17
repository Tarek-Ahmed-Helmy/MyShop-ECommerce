using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using Stripe;

namespace myshop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public OrderVM OrderVM { get; set; }
    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetData()
    {
        IEnumerable<Order> orders;
        orders = _unitOfWork.Order.GetAll(includeEntity: "ApplicationUser");
        return Json(new { data = orders });
    }

    public IActionResult Details(int? id)
    {
        OrderVM orderVM = new OrderVM()
        {
            Order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == id, includeEntity: "ApplicationUser"),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderId == id, includeEntity: "Product")
        };
        return View(orderVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderDetails() 
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == OrderVM.Order.Id);
        order.FullName = OrderVM.Order.FullName;
        order.Address = OrderVM.Order.Address;
        order.City = OrderVM.Order.City;
        order.PhoneNumber = OrderVM.Order.PhoneNumber;
        
        if(OrderVM.Order.Carrier != null)
        {
            order.Carrier = OrderVM.Order.Carrier;
        }
        if (OrderVM.Order.TrackingNumber != null)
        {
            order.TrackingNumber = OrderVM.Order.TrackingNumber;
        }
        _unitOfWork.Order.Update(order);
        _unitOfWork.Complete();
        TempData["UpdateMsg"] = "Order details updated successfully";
        return RedirectToAction("Details", new { id = order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartProccess() 
    {
        _unitOfWork.Order.UpdateOrderStatus(OrderVM.Order.Id, SD.processing, null);
        _unitOfWork.Complete();

        TempData["UpdateMsg"] = "Order status updated successfully";
        return RedirectToAction("Details", new { id = OrderVM.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartShip()
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == OrderVM.Order.Id);
        order.TrackingNumber = OrderVM.Order.TrackingNumber;
        order.Carrier = OrderVM.Order.Carrier;
        order.OrderStatus = SD.Shipped;
        order.ShippingDate = DateTime.Now;

        _unitOfWork.Order.Update(order);
        _unitOfWork.Complete();

        TempData["UpdateMsg"] = "Order has shipped successfully";
        return RedirectToAction("Details", new { id = OrderVM.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder()
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == OrderVM.Order.Id);
        if(order.PaymentStatus == SD.Approved)
        {
            var option = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = order.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(option);

            _unitOfWork.Order.UpdateOrderStatus(order.Id, SD.Cancelled, SD.Refunded);
        }
        else
        {
            _unitOfWork.Order.UpdateOrderStatus(order.Id, SD.Cancelled, null);
        }

        TempData["UpdateMsg"] = "Order has canceld successfully";
        return RedirectToAction("Details", new { id = OrderVM.Order.Id });
    }
}
