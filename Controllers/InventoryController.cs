using DoAnCoSo.Helpers;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IWarehouseService _warehouseService;

        public InventoryController(IInventoryService inventoryService, IWarehouseService warehouseService)
        {
            _inventoryService = inventoryService;
            _warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index(int? warehouseId)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            ViewBag.Warehouses = await _warehouseService.GetAllActiveWarehousesAsync();
            ViewBag.SelectedWarehouseId = warehouseId;

            var inventory = await _inventoryService.GetFullInventoryAsync(warehouseId);
            return View(inventory);
        }
    }
}