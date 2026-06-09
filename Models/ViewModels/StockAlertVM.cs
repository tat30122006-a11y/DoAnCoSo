namespace DoAnCoSo.Models.ViewModels
{
    public class StockAlertVM
    {
        public string WarehouseName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int CurrentStock { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public string StockStatusText { get; set; } = null!;
    }
}