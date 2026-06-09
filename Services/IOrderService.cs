using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface IOrderService
    {
        Task<(bool IsSuccess, string Message)> ConfirmInboundOrderAsync(int orderId, int processedByUserId);
        Task<(bool IsSuccess, string Message)> ConfirmOutboundOrderAsync(int orderId, int processedByUserId);
        Task<(bool IsSuccess, string Message)> UpdateTransferStatusAsync(int transferId, string newStatus, int processedByUserId, Dictionary<int, int>? actualQuantities = null);
    }
}