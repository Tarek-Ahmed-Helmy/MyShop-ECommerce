﻿using myshop.DataAccess.Data;
using myshop.Entities.Repositories;

namespace myshop.DataAccess.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICartItemRepository CartItem { get; private set; }
    public IOrderRepository Order { get; private set; }
    public IOrderDetailRepository OrderDetail { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Category = new CategoryRepository(context);
        Product = new ProductRepository(context);
        CartItem = new CartItemRepository(context);
        Order = new OrderRepository(context);
        OrderDetail = new OrderDetailRepository(context);
        ApplicationUser = new ApplicationUserRepository(context);
    }

    public int Complete()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
