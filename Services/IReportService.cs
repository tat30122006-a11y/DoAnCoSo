using DoAnCoSo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface IReportService
    {
        Task<List<DailyLedgerVM>> GetDailyLedgerAsync(DateTime fromDate, DateTime toDate);
        Task<List<StockLossVM>> GetStockLossesAsync();
        Task<List<AuditLogVM>> GetSystemAuditLogsAsync();
    }
}