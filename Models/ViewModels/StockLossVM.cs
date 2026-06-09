namespace DoAnCoSo.Models.ViewModels
{
    public class StockLossVM
    {
        public int Id { get; set; }
        public string TransferCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public int LossQty { get; set; }
        public string? Reason { get; set; }
        public bool ResolvedStatus { get; set; }
        public string ResolvedStatusText => ResolvedStatus ? "Đã xử lý" : "Chờ xử lý";
    }
}