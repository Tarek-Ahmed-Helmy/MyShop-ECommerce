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
        var orders = _unitOfWork.Order.GetAll(includeEntity: "ApplicationUser");
        return Json(new { data = orders });
    }

    public IActionResult Details(int? id)
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == id, includeEntity: "ApplicationUser");
        var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderId == id, includeEntity: "Product");
        OrderVM orderVM = new OrderVM()
        {
            Id = order.Id,
            FullName = order.ApplicationUser.FullName,
            Address = order.ApplicationUser.Address,
            City = order.ApplicationUser.City,
            PhoneNumber = order.ApplicationUser.PhoneNumber,
            Email = order.ApplicationUser.Email,
            TrackingNumber = order.TrackingNumber,
            Carrier = order.Carrier,
            OrderDate = order.OrderDate,
            ShippingDate = order.ShippingDate,
            SessionId = order.SessionId,
            PaymentIntentId = order.PaymentIntentId,
            PaymentDate = order.PaymentDate,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus,
            TotalAmount = order.TotalAmount,
            OrderDetails = orderDetails
        };
        return View(orderVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderDetails()
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == OrderVM.Id, includeEntity: "ApplicationUser");
        order.ApplicationUser.FullName = OrderVM.FullName;
        order.ApplicationUser.Address = OrderVM.Address;
        order.ApplicationUser.City = OrderVM.City;
        order.ApplicationUser.PhoneNumber = OrderVM.PhoneNumber;

        if (OrderVM.Carrier != null)
        {
            order.Carrier = OrderVM.Carrier;
        }
        if (OrderVM.TrackingNumber != null)
        {
            order.TrackingNumber = OrderVM.TrackingNumber;
        }
        _unitOfWork.Order.Update(order);
        _unitOfWork.Complete();
        TempData["UpdateMsg"] = "Order details updated successfully";
        return RedirectToAction("Details", "Order", new { id = order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartProccess()
    {
        _unitOfWork.Order.UpdateOrderStatus(OrderVM.Id, SD.processing, null);
        _unitOfWork.Complete();

        TempData["UpdateMsg"] = "Order status has updated successfully";
        return RedirectToAction("Details", "Order", new { id = OrderVM.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartShip()
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == OrderVM.Id);
        order.TrackingNumber = OrderVM.TrackingNumber;
        order.Carrier = OrderVM.Carrier;
        order.OrderStatus = SD.Shipped;
        order.ShippingDate = DateTime.Now;

        _unitOfWork.Order.Update(order);
        _unitOfWork.Complete();

        TempData["UpdateMsg"] = "Order has shipped successfully";
        return RedirectToAction("Details", "Order", new { id = OrderVM.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder()
    {
        var order = _unitOfWork.Order.GetFirstOrDefault(o => o.Id == OrderVM.Id);
        if (order.PaymentStatus == SD.Approved)
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
            _unitOfWork.Order.UpdateOrderStatus(order.Id, SD.Cancelled, SD.Cancelled);
        }
        _unitOfWork.Complete();

        TempData["UpdateMsg"] = "Order has canceld successfully";
        return RedirectToAction("Details", new { id = OrderVM.Id });
    }
}