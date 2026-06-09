using DoAnCoSo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehouseVM>> GetAllActiveWarehousesAsync();
        Task<WarehouseVM?> GetWarehouseByIdAsync(int id);
    }
}