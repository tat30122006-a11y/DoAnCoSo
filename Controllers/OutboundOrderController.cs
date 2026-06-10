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
    public class OutboundOrderController : Controller
    {
        private readonly QuanLyKhoContext _context;
        private readonly IOrderService _orderService;

        public OutboundOrderController(QuanLyKhoContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        // ========================================================
        // 1. MÀN HÌNH DANH SÁCH PHIẾU XUẤT
        // ========================================================
        public async Task<IActionResult> Index()
        {
            var orders = await _context.OutboundOrders
                .Include(o => o.Warehouse)
                .Include(o => o.Customer) // Xuất cho Khách hàng
                .Include(o => o.CreatedByNavigation)
                .OrderByDescending(o => o.CreatedDate)
                .Select(o => new OutboundOrderVM
                {
                    Id = o.Id,
                    OutboundCode = o.OutboundCode,
                    WarehouseName = o.Warehouse.Name,
                    CustomerName = o.Customer.Name,
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
            ViewBag.Warehouses = _context.Warehouses.Where(w => w.IsActive).ToList();
            ViewBag.Customers = _context.Customers.Where(c => c.IsActive).ToList();
            ViewBag.Products = _context.Products.Where(p => p.IsActive).ToList();

            return View();
        }

        // ========================================================
        // 3. XỬ LÝ LƯU PHIẾU NHÁP VÀO DATABASE (POST)
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int warehouseId, int customerId, string? note, int[] productIds, int[] quantities, decimal[] prices)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            if (productIds == null || productIds.Length == 0)
            {
                TempData.SetErrorMessage("Phải chọn ít nhất 1 sản phẩm để xuất kho!");
                return RedirectToAction(nameof(Create));
            }

            // Sinh mã phiếu xuất: OUT-NgàyTháng-MãNgẫuNhiên
            string newCode = $"OUT-{DateTime.Now:yyMMdd}-{new Random().Next(1000, 9999)}";

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lưu Master
                var order = new OutboundOrder
                {
                    OutboundCode = newCode,
                    WarehouseId = warehouseId,
                    CustomerId = customerId,
                    Status = SystemConstants.OrderStatus.OutboundPending, // "chờ xuất"
                    CreatedBy = userSession.Id,
                    Note = note
                };

                _context.OutboundOrders.Add(order);
                await _context.SaveChangesAsync();

                // Lưu Details
                for (int i = 0; i < productIds.Length; i++)
                {
                    _context.OutboundOrderDetails.Add(new OutboundOrderDetail
                    {
                        OutboundOrderId = order.Id,
                        ProductId = productIds[i],
                        Quantity = quantities[i],
                        Price = prices[i]
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData.SetSuccessMessage($"Đã tạo phiếu xuất {newCode} thành công. Hãy kiểm tra và duyệt để xuất hàng!");
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
            var order = await _context.OutboundOrders
                .Include(o => o.Warehouse)
                .Include(o => o.Customer)
                .Include(o => o.CreatedByNavigation)
                .Include(o => o.ProcessedByNavigation)
                .Include(o => o.OutboundOrderDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            ViewBag.OrderDetails = order.OutboundOrderDetails.Select(d => new OrderDetailVM
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
        // 5. XÁC NHẬN DUYỆT PHIẾU XUẤT KHO (GỌI STORED PROCEDURE)
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            // Service gọi Stored Procedure chống âm kho
            var result = await _orderService.ConfirmOutboundOrderAsync(id, userSession.Id);

            if (result.IsSuccess)
            {
                TempData.SetSuccessMessage(result.Message);
            }
            else
            {
                // Nếu xuất quá số lượng tồn kho, SP sẽ văng lỗi lên đây
                TempData.SetErrorMessage(result.Message);
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}