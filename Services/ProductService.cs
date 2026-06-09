using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class ProductService : IProductService
    {
        private readonly QuanLyKhoContext _context;

        public ProductService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<List<ProductVM>> GetAllActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Select(p => new ProductVM
                {
                    Id = p.Id,
                    SKU = p.Sku,
                    Barcode = p.Barcode,
                    Name = p.Name,
                    Category = p.Category
                })
                .ToListAsync();
        }

        public async Task<ProductVM?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            return new ProductVM
            {
                Id = product.Id,
                SKU = product.Sku,
                Barcode = product.Barcode,
                Name = product.Name,
                Category = product.Category
            };
        }
    }
}