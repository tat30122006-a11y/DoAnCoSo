using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class OrderService : IOrderService
    {
        private readonly QuanLyKhoContext _context;

        public OrderService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message)> ConfirmInboundOrderAsync(int orderId, int processedByUserId)
        {
            var isSuccessParam = new SqlParameter("@IsSuccess", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_ConfirmInboundOrder @InboundOrderId, @ProcessedBy, @IsSuccess OUT, @Message OUT",
                new SqlParameter("@InboundOrderId", orderId),
                new SqlParameter("@ProcessedBy", processedByUserId),
                isSuccessParam, messageParam);

            return ((bool)isSuccessParam.Value, (string)messageParam.Value);
        }

        public async Task<(bool IsSuccess, string Message)> ConfirmOutboundOrderAsync(int orderId, int processedByUserId)
        {
            var isSuccessParam = new SqlParameter("@IsSuccess", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_ConfirmOutboundOrder @OutboundOrderId, @ProcessedBy, @IsSuccess OUT, @Message OUT",
                new SqlParameter("@OutboundOrderId", orderId),
                new SqlParameter("@ProcessedBy", processedByUserId),
                isSuccessParam, messageParam);

            return ((bool)isSuccessParam.Value, (string)messageParam.Value);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTransferStatusAsync(int transferId, string newStatus, int processedByUserId, Dictionary<int, int>? actualQuantities = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.TransferOrders
                    .Include(t => t.TransferOrderDetails)
                    .FirstOrDefaultAsync(t => t.Id == transferId);

                if (order == null) return (false, "Không tìm thấy phiếu chuyển kho.");

                // Map số lượng thực nhận từ UI xuống nếu trạng thái là "đã nhận"
                if (newStatus == SystemConstants.OrderStatus.Received && actualQuantities != null)
                {
                    foreach (var detail in order.TransferOrderDetails)
                    {
                        if (actualQuantities.TryGetValue(detail.ProductId, out int actualQty))
                        {
                            detail.ActualQty = actualQty;
                        }
                    }
                }

                order.Status = newStatus;
                order.ProcessedBy = processedByUserId;
                order.ProcessedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Cập nhật trạng thái luân chuyển thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}