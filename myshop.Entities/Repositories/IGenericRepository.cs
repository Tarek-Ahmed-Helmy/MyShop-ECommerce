using System.Linq.Expressions;

namespace myshop.Entities.Repositories;

public interface IGenericRepository<T> where T : class
{
    // _context.Entity.Include("Entity2").ToList();
    // _context.Entity.Where("Expression").ToList();
    IEnumerable<T> GetAll(Expression<Func<T, bool>>? perdicate = null, string? includeEntity = null);

    // _context.Entity.Include("Entity2").SingleOrDefault();
    // _context.Entity.Where("Expression").SingleOrDefault();
    T GetFirstOrDefault(Expression<Func<T, bool>>? perdicate = null, string? includeEntity = null);

    // _context.Entity.Add(item);
    void Add(T item);

    // _context.Entity.Remove(item);
    void Remove(T item);

    void RemoveRange(IEnumerable<T> items);
}
