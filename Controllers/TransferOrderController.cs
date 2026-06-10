using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class TransferOrderController : Controller
    {
        private readonly QuanLyKhoContext _context;
        private readonly IOrderService _orderService;

        public TransferOrderController(QuanLyKhoContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        // ========================================================
        // 1. MÀN HÌNH DANH SÁCH LỆNH ĐIỀU CHUYỂN
        // ========================================================
        public async Task<IActionResult> Index()
        {
            var orders = await _context.TransferOrders
                .Include(o => o.FromWarehouse)
                .Include(o => o.ToWarehouse)
                .Include(o => o.CreatedByNavigation)
                .OrderByDescending(o => o.CreatedDate)
                .Select(o => new TransferOrderVM
                {
                    Id = o.Id,
                    TransferCode = o.TransferCode,
                    FromWarehouseName = o.FromWarehouse.Name,
                    ToWarehouseName = o.ToWarehouse.Name,
                    Status = o.Status,
                    CreatedBy = o.CreatedByNavigation.FullName,
                    CreatedDate = o.CreatedDate
                })
                .ToListAsync();

            return View(orders);
        }

        // ========================================================
        // 2. MÀN HÌNH TẠO LỆNH (GET)
        // ========================================================
        public IActionResult Create()
        {
            // Chỉ cần lấy danh sách Kho và Sản phẩm (không có NCC hay Khách hàng)
            ViewBag.Warehouses = _context.Warehouses.Where(w => w.IsActive).ToList();
            ViewBag.Products = _context.Products.Where(p => p.IsActive).ToList();

            return View();
        }

        // ========================================================
        // 3. LƯU LỆNH ĐIỀU CHUYỂN NHÁP (POST)
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int fromWarehouseId, int toWarehouseId, int[] productIds, int[] quantities)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            // Rào lỗi: Kho xuất và Kho nhập không được phép trùng nhau
            if (fromWarehouseId == toWarehouseId)
            {
                TempData.SetErrorMessage("Kho xuất và Kho nhập phải khác nhau!");
                return RedirectToAction(nameof(Create));
            }

            if (productIds == null || productIds.Length == 0)
            {
                TempData.SetErrorMessage("Phải chọn ít nhất 1 sản phẩm để luân chuyển!");
                return RedirectToAction(nameof(Create));
            }

            string newCode = $"TRF-{DateTime.Now:yyMMdd}-{new Random().Next(1000, 9999)}";

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new TransferOrder
                {
                    TransferCode = newCode,
                    FromWarehouseId = fromWarehouseId,
                    ToWarehouseId = toWarehouseId,
                    Status = SystemConstants.OrderStatus.Pending, // "chờ duyệt"
                    CreatedBy = userSession.Id
                };

                _context.TransferOrders.Add(order);
                await _context.SaveChangesAsync();

                for (int i = 0; i < productIds.Length; i++)
                {
                    _context.TransferOrderDetails.Add(new TransferOrderDetail
                    {
                        TransferOrderId = order.Id,
                        ProductId = productIds[i],
                        RequestedQty = quantities[i]
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData.SetSuccessMessage($"Đã tạo lệnh điều chuyển {newCode} thành công. Chờ kho xuất hàng xác nhận!");
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
        // 4. XEM CHI TIẾT LỆNH ĐIỀU CHUYỂN
        // ========================================================
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.TransferOrders
                .Include(o => o.FromWarehouse)
                .Include(o => o.ToWarehouse)
                .Include(o => o.CreatedByNavigation)
                .Include(o => o.ProcessedByNavigation)
                .Include(o => o.TransferOrderDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Đẩy chi tiết ra View (có bao gồm cả số lượng gửi và số lượng thực nhận)
            ViewBag.Details = order.TransferOrderDetails.Select(d => new TransferOrderDetailVM
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductName = d.Product.Name,
                SKU = d.Product.Sku,
                RequestedQty = d.RequestedQty,
                ActualQty = d.ActualQty
            }).ToList();

            return View(order);
        }

        // ========================================================
        // 5. BƯỚC 1: KHO XUẤT XÁC NHẬN GỬI HÀNG ĐI
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> StartTransfer(int id)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            var result = await _orderService.UpdateTransferStatusAsync(id, SystemConstants.OrderStatus.InTransit, userSession.Id);

            if (result.IsSuccess)
                TempData.SetSuccessMessage("Hàng hóa đã bắt đầu được luân chuyển. Đã trừ tồn kho tại kho xuất!");
            else
                TempData.SetErrorMessage(result.Message); // Nếu kho xuất không đủ hàng sẽ báo lỗi

            return RedirectToAction(nameof(Details), new { id });
        }

        // ========================================================
        // 6. BƯỚC 2: KHO NHẬP XÁC NHẬN ĐÃ NHẬN HÀNG VÀ CHỐT SỐ LƯỢNG
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> ConfirmReceive(int id, int[] productIds, int[] actualQties)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null) return RedirectToAction("Login", "Auth");

            // Gom 2 mảng (ID sản phẩm và Số lượng thực nhận) thành 1 Dictionary để gửi xuống Service
            var actualQuantitiesDict = new Dictionary<int, int>();
            for (int i = 0; i < productIds.Length; i++)
            {
                actualQuantitiesDict[productIds[i]] = actualQties[i];
            }

            var result = await _orderService.UpdateTransferStatusAsync(id, SystemConstants.OrderStatus.Received, userSession.Id, actualQuantitiesDict);

            if (result.IsSuccess)
                TempData.SetSuccessMessage("Kho đến đã nhận hàng. Hệ thống tự động cập nhật tồn kho và ghi nhận hao hụt (nếu có)!");
            else
                TempData.SetErrorMessage(result.Message);

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}