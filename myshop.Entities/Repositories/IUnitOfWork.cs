﻿namespace myshop.Entities.Repositories;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IOrderRepository Order { get; }
    IOrderDetailRepository OrderDetail { get; }
    IApplicationUserRepository ApplicationUser { get; }

    int Complete();
}
