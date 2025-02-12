using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Repositories;
using System.Linq.Expressions;

namespace myshop.DataAccess.Implementation;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private DbSet<T> _dbSet;
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public void Add(T item)
    {
        _dbSet.Add(item);
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? perdicate = null, string? includeEntity = null)
    {
        IQueryable<T> query = _dbSet;
        if (perdicate != null)
        {
            query = query.Where(perdicate);
        }
        if (includeEntity != null)
        {
            // _context.Products.Include("Category,Logos,Users)
            foreach (var item in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(item);
            }
        }
        return query.ToList();
    }

    public T GetFirstOrDefault(Expression<Func<T, bool>>? perdicate = null, string? includeEntity = null)
    {
        IQueryable<T> query = _dbSet;
        if (perdicate != null)
        {
            query = query.Where(perdicate);
        }
        if (includeEntity != null)
        {
            // _context.Products.Include("Category,Logos,Users)
            foreach (var item in includeEntity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(item);
            }
        }
        return query.SingleOrDefault();
    }

    public void Remove(T item)
    {
        _dbSet.Remove(item);
    }

    public void RemoveRange(IEnumerable<T> items)
    {
        _dbSet.RemoveRange(items);
    }
}
