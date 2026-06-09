using DoAnCoSo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerVM>> GetAllActiveCustomersAsync();
    }
}