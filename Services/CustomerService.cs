using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly QuanLyKhoContext _context;

        public CustomerService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerVM>> GetAllActiveCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .Select(c => new CustomerVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }
    }
}