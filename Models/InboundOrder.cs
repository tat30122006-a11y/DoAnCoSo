using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class InboundOrder
{
    public int Id { get; set; }

    public string InboundCode { get; set; } = null!;

    public int WarehouseId { get; set; }

    public int SupplierId { get; set; }

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? Note { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<InboundOrderDetail> InboundOrderDetails { get; set; } = new List<InboundOrderDetail>();

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
