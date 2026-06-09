using System;

namespace DoAnCoSo.Models.ViewModels
{
    public class DailyLedgerVM
    {
        public DateTime TransactionDate { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public int QuantityChanged { get; set; }
        public int BeforeQuantity { get; set; }
        public int AfterQuantity { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}