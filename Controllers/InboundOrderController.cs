using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class InboundOrderController : Controller
    {
        private readonly QuanLyKhoContext _context;
        private readonly IOrderService _orderService;

        public InboundOrderController(QuanLyKhoContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        // ========================================================
        // 1. MÀN HÌNH DANH SÁCH PHIẾU NHẬP
        // ========================================================
        public async Task<IActionResult> Index()
        {
            // Join các bảng để lấy Tên Kho, Tên Nhà Cung Cấp, Tên Người Lập
            var orders = await _context.InboundOrders
                .Include(o => o.Warehouse)
                .Include(o => o.Supplier)
                .Include(o => o.CreatedByNavigation)
                .OrderByDescending(o => o.CreatedDate)
                .Select(o => new InboundOrderVM
                {
                    Id = o.Id,
                    InboundCode = o.InboundCode,
                    WarehouseName = o.Warehouse.Name,
                    SupplierName = o.Supplier.Name,
                    Status = o.Status,
                    CreatedBy = o.CreatedByNavigation.FullName,
                    CreatedDate = o.CreatedDate
                })
                .ToListAsync();

            return View(orders);
        }

        // ========================================================
        // 2. MÀN HÌNH TẠO PHIẾU NHÁP (GET)
        // ========================================================
        public IActionResult Create()
        {
            // Truyền danh sách Kho, NCC, Sản phẩm (đang hoạt động) sang View để làm thẻ Select
            ViewBag.Warehouses = _context.Warehouses.Where(w => w.IsActive).ToList();
            ViewBag.Suppliers = _context.Suppliers.Where(s => s.IsActive).ToList();
            ViewBag.Products = _context.Products.Where(p => p.IsActive).ToList();

            return View();
        }

        // ========================================================
        // 3. XỬ LÝ LƯU PHIẾU NHÁP VÀO DATABASE (POST)
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int warehouseId, int supplierId, string? note, int[] productIds, int[] quantities, decimal[] prices)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            // Rào lỗi: Không chọn sản phẩm nào
            if (productIds == null || productIds.Length == 0)
            {
                TempData.SetErrorMessage("Phải chọn ít nhất 1 sản phẩm để nhập kho!");
                return RedirectToAction(nameof(Create));
            }

            // Sinh mã phiếu tự động (Ví dụ: INB-260609-1234)
            string newCode = $"INB-{DateTime.Now:yyMMdd}-{new Random().Next(1000, 9999)}";

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Bước 3.1: Lưu thông tin chung của phiếu
                var order = new InboundOrder
                {
                    InboundCode = newCode,
                    WarehouseId = warehouseId,
                    SupplierId = supplierId,
                    Status = SystemConstants.OrderStatus.InboundPending, // Mặc định là "chờ nhập"
                    CreatedBy = userSession.Id,
                    Note = note
                };

                _context.InboundOrders.Add(order);
                await _context.SaveChangesAsync(); // Phải Save để SQL sinh ra order.Id

                // Bước 3.2: Lưu chi tiết từng mặt hàng vào phiếu
                for (int i = 0; i < productIds.Length; i++)
                {
                    _context.InboundOrderDetails.Add(new InboundOrderDetail
                    {
                        InboundOrderId = order.Id, // Lấy ID vừa sinh ở trên
                        ProductId = productIds[i],
                        Quantity = quantities[i],
                        Price = prices[i]
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData.SetSuccessMessage($"Đã tạo phiếu nhập {newCode} thành công. Hãy kiểm tra lại và duyệt để cất vào kho!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData.SetErrorMessage("Lỗi hệ thống: " + ex.Message);
                return RedirectToAction(nameof(Create));
            }
        }

        // ========================================================
        // 4. MÀN HÌNH XEM CHI TIẾT PHIẾU
        // ========================================================
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.InboundOrders
                .Include(o => o.Warehouse)
                .Include(o => o.Supplier)
                .Include(o => o.CreatedByNavigation)
                .Include(o => o.ProcessedByNavigation)
                .Include(o => o.InboundOrderDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Đóng gói danh sách chi tiết hàng hóa đẩy sang View
            ViewBag.OrderDetails = order.InboundOrderDetails.Select(d => new OrderDetailVM
            {
                ProductId = d.ProductId,
                ProductName = d.Product.Name,
                SKU = d.Product.Sku,
                Quantity = d.Quantity,
                Price = d.Price
            }).ToList();

            return View(order);
        }

        // ========================================================
        // 5. XÁC NHẬN DUYỆT PHIẾU NHẬP (GỌI STORED PROCEDURE)
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            // Truyền ID phiếu và ID người duyệt xuống Service để gọi lệnh EXEC SQL
            var result = await _orderService.ConfirmInboundOrderAsync(id, userSession.Id);

            if (result.IsSuccess)
            {
                TempData.SetSuccessMessage(result.Message);
            }
            else
            {
                TempData.SetErrorMessage(result.Message);
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}