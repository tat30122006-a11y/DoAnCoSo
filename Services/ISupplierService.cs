using DoAnCoSo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface ISupplierService
    {
        Task<List<SupplierVM>> GetAllActiveSuppliersAsync();
    }
}