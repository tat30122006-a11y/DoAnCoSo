using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly QuanLyKhoContext _context;

        public WarehouseService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<List<WarehouseVM>> GetAllActiveWarehousesAsync()
        {
            // Trích xuất dữ liệu từ DB, chỉ lấy các kho đang hoạt động và map thẳng sang ViewModel
            return await _context.Warehouses
                .Where(w => w.IsActive)
                .Select(w => new WarehouseVM
                {
                    Id = w.Id,
                    Name = w.Name,
                    Address = w.Address,
                    IsActive = w.IsActive
                })
                .ToListAsync();
        }

        public async Task<WarehouseVM?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null) return null;

            return new WarehouseVM
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Address = warehouse.Address,
                IsActive = warehouse.IsActive
            };
        }
    }
}