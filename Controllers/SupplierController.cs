using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using DoAnCoSo.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly QuanLyKhoContext _context;

        public SupplierController(ISupplierService supplierService, QuanLyKhoContext context)
        {
            _supplierService = supplierService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllActiveSuppliersAsync();
            return View(suppliers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var supplier = new Supplier
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                IsActive = true
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã thêm nhà cung cấp: {supplier.Name}");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            var model = new SupplierVM
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                IsActive = supplier.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierVM model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            supplier.Name = model.Name;
            supplier.Phone = model.Phone;
            supplier.Email = model.Email;
            supplier.Address = model.Address;
            supplier.IsActive = model.IsActive;

            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã cập nhật nhà cung cấp: {supplier.Name}");
            return RedirectToAction(nameof(Index));
        }
    }
}