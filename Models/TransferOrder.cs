using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class TransferOrder
{
    public int Id { get; set; }

    public string TransferCode { get; set; } = null!;

    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Warehouse FromWarehouse { get; set; } = null!;

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual ICollection<StockLoss> StockLosses { get; set; } = new List<StockLoss>();

    public virtual Warehouse ToWarehouse { get; set; } = null!;

    public virtual ICollection<TransferOrderDetail> TransferOrderDetails { get; set; } = new List<TransferOrderDetail>();
}
