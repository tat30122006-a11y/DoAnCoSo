using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly QuanLyKhoContext _context;

        public SupplierService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierVM>> GetAllActiveSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.IsActive)
                .Select(s => new SupplierVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    Phone = s.Phone,
                    Email = s.Email,
                    Address = s.Address,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }
    }
}