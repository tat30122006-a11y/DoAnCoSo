using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // ========================================================
        // 1. BÁO CÁO SỔ CÁI BIẾN ĐỘNG KHO (DAILY LEDGER)
        // ========================================================
        public async Task<IActionResult> DailyLedger(DateTime? fromDate, DateTime? toDate)
        {
            // Nếu thủ kho không chọn ngày, mặc định lấy từ ngày mùng 1 đầu tháng đến hôm nay
            var today = DateTime.Today;
            var start = fromDate ?? new DateTime(today.Year, today.Month, 1);
            var end = toDate ?? today.AddDays(1).AddSeconds(-1); // Hết ngày hôm nay

            // Giữ lại bộ lọc ngày truyền sang ô Input ngoài giao diện View
            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");

            var ledgerData = await _reportService.GetDailyLedgerAsync(start, end);
            return View(ledgerData);
        }

        // ========================================================
        // 2. DANH SÁCH THEO DÕI HÀNG HAO HỤT / THẤT THOÁT
        // ========================================================
        public async Task<IActionResult> StockLoss()
        {
            var lossData = await _reportService.GetStockLossesAsync();
            return View(lossData);
        }

        // ========================================================
        // 3. NHẬT KÝ GIÁM SÁT HỆ THỐNG (AUDIT LOG)
        // ========================================================
        public async Task<IActionResult> AuditLog()
        {
            var auditData = await _reportService.GetSystemAuditLogsAsync();
            return View(auditData);
        }
    }
}