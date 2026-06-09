using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    // Bắt buộc đăng nhập mới được xem trang này (Tạm thời có thể comment lại nếu chưa làm tính năng Login)
    // [Authorize] 
    public class DashboardController : Controller
    {
        private readonly IInventoryService _inventoryService;

        // Tiêm (Inject) Service vào Controller thông qua Constructor
        public DashboardController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Action hiển thị trang chủ Dashboard
        public async Task<IActionResult> Index()
        {
            // 1. Gọi Service để lấy dữ liệu (Service đã tự động map sang StockAlertVM cho bạn)
            var alertData = await _inventoryService.GetDashboardAlertsAsync();

            // 2. Ném dữ liệu ra View để hiển thị
            return View(alertData);
        }
    }
}