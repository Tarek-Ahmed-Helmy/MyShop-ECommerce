using myshop.Entities.Models;

namespace myshop.Entities.ViewModels;

public class OrderVM
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ShippingDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? PaymentStatus { get; set; }
    public string? OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
}
