using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;

namespace myshop.DataAccess.Implementation;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _context;
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }

    public void UpdateOrderStatus(int orderId, string orderStatus, string paymentStatus)
    {
        Order order = _context.Orders.Find(orderId);
        if (order != null)
        {
            order.OrderStatus = orderStatus;
            if(paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
                order.PaymentDate = DateTime.Now;
            }
        }
    }
}
