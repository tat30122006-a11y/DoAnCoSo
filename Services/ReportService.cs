using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class ReportService : IReportService
    {
        private readonly QuanLyKhoContext _context;

        public ReportService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<List<DailyLedgerVM>> GetDailyLedgerAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.InventoryTransactions
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new DailyLedgerVM
                {
                    TransactionDate = x.CreatedDate,
                    WarehouseName = x.Warehouse.Name,
                    ProductName = x.Product.Name,
                    SKU = x.Product.Sku,
                    TransactionType = x.TransactionType,
                    ReferenceCode = x.ReferenceCode,
                    QuantityChanged = x.QuantityChanged,
                    BeforeQuantity = x.BeforeQuantity,
                    AfterQuantity = x.AfterQuantity,
                    CreatedBy = x.CreatedBy.ToString()
                })
                .ToListAsync();
        }

        public async Task<List<StockLossVM>> GetStockLossesAsync()
        {
            return await _context.StockLosses
                .Include(x => x.TransferOrder)
                .Include(x => x.Product)
                .OrderByDescending(x => x.Id)
                .Select(x => new StockLossVM
                {
                    Id = x.Id,
                    TransferCode = x.TransferOrder.TransferCode,
                    ProductName = x.Product.Name,
                    SKU = x.Product.Sku,
                    LossQty = x.LossQty,
                    Reason = x.Reason ?? "", // [Bổ sung] Chặn luôn cảnh báo null cho Reason
                    ResolvedStatus = x.ResolvedStatus
                })
                .ToListAsync();
        }

        public async Task<List<AuditLogVM>> GetSystemAuditLogsAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new AuditLogVM
                {
                    Id = x.Id,
                    Username = x.User.Username, // [Đã sửa] Gọi vòng qua bảng User để lấy tên đăng nhập
                    Action = x.Action,
                    TableName = x.TableName,
                    RecordId = x.RecordId,
                    Timestamp = x.Timestamp
                })
                .ToListAsync();
        }
    }
}