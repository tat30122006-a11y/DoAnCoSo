using System;

namespace DoAnCoSo.Models.ViewModels
{
    public class OutboundOrderVM
    {
        public int Id { get; set; }
        public string OutboundCode { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? ProcessedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? Note { get; set; }
    }
}