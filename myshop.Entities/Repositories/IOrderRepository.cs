using myshop.Entities.Models;

namespace myshop.Entities.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    void Update(Order order);
    void UpdateOrderStatus(int orderId, string orderStatus, string? paymentStatus);
}
