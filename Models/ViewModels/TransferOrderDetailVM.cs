namespace DoAnCoSo.Models.ViewModels
{
    public class TransferOrderDetailVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public int RequestedQty { get; set; }
        public int? ActualQty { get; set; }
        public int LossQty => ActualQty.HasValue ? (RequestedQty - ActualQty.Value) : 0;
    }
}