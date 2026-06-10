using DoAnCoSo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface IInventoryService
    {
        Task<List<StockAlertVM>> GetDashboardAlertsAsync();
        Task<List<InventoryVM>> GetFullInventoryAsync(int? warehouseId);
    }
}