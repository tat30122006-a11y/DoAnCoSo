using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using DoAnCoSo.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly QuanLyKhoContext _context;

        public WarehouseController(IWarehouseService warehouseService, QuanLyKhoContext context)
        {
            _warehouseService = warehouseService;
            _context = context;
        }

        // GET: /Warehouse
        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseService.GetAllActiveWarehousesAsync();
            return View(warehouses);
        }

        // GET: /Warehouse/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var warehouse = new Warehouse
            {
                Name = model.Name,
                Address = model.Address,
                IsActive = true
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã thêm kho mới: {warehouse.Name}");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Warehouse/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null) return NotFound();
            return View(warehouse);
        }

        // POST: /Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseVM model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null) return NotFound();

            warehouse.Name = model.Name;
            warehouse.Address = model.Address;
            warehouse.IsActive = model.IsActive;

            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã cập nhật thông tin kho: {warehouse.Name}");
            return RedirectToAction(nameof(Index));
        }
    }
}