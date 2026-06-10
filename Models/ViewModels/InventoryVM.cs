using System;

namespace DoAnCoSo.Models.ViewModels
{
    public class InventoryVM
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int ProductId { get; set; }
        public string SKU { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}