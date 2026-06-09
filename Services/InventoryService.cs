using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly QuanLyKhoContext _context;

        public InventoryService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<List<StockAlertVM>> GetDashboardAlertsAsync()
        {
            var data = await _context.VInventoryDashboardAlerts.ToListAsync();

            return data.Select(x => new StockAlertVM
            {
                WarehouseName = x.WarehouseName,
                SKU = x.Sku,
                ProductName = x.ProductName,
                CurrentStock = x.CurrentStock, // [Đã sửa] Xóa bỏ ?? 0 vì nó vốn dĩ đã là int
                MinQuantity = x.MinQuantity,
                MaxQuantity = x.MaxQuantity,
                StockStatusText = x.StockStatusText ?? "Chưa xác định" // [Đã sửa] Xử lý triệt để cảnh báo Null
            }).ToList();
        }
    }
}