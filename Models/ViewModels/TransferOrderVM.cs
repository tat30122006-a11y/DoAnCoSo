using System;

namespace DoAnCoSo.Models.ViewModels
{
    public class TransferOrderVM
    {
        public int Id { get; set; }
        public string TransferCode { get; set; } = null!;
        public string FromWarehouseName { get; set; } = null!;
        public string ToWarehouseName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}