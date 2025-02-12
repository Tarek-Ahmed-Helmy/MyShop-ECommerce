using System.Linq.Expressions;

namespace myshop.Entities.Repositories;

public interface IGenericRepository<T> where T : class
{
    // _context.Entity1.Include("Entity2").ToList();
    // _context.Entity1.Where("Expression").ToList();
    IEnumerable<T> GetAll(Expression<Func<T, bool>>? perdicate = null, string? includeEntity = null);
    // _context.Entity1.Include("Entity2").SingleOrDefault();
    // _context.Entity1.Where("Expression").SingleOrDefault();
    T GetFirstOrDefault(Expression<Func<T, bool>>? perdicate = null, string? includeEntity = null);
    // _context.Entity1.Add(item);
    void Add(T item);
    // _context.Entity1.Remove(item);
    void Remove(T item);
    void RemoveRange(IEnumerable<T> items);
}
