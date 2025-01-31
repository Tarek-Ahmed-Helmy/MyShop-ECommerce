using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Implementation
{
    internal class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductInDb = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (ProductInDb != null) 
            {
                ProductInDb.ProductName = product.ProductName;
                ProductInDb.ProductDescription = product.ProductDescription;
                ProductInDb.Price = product.Price;
                ProductInDb.ImgPath = product.ImgPath;
                ProductInDb.CategoryId = product.CategoryId;
                //ProductInDb.UpdatedAt = DateTime.Now;
            }
        }
    }
}
