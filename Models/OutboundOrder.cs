using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class OutboundOrder
{
    public int Id { get; set; }

    public string OutboundCode { get; set; } = null!;

    public int WarehouseId { get; set; }

    public int CustomerId { get; set; }

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? Note { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OutboundOrderDetail> OutboundOrderDetails { get; set; } = new List<OutboundOrderDetail>();

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
