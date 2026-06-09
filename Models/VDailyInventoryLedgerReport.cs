using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class VDailyInventoryLedgerReport
{
    public DateOnly? TransactionDate { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; } = null!;

    public string ReferenceCode { get; set; } = null!;

    public int QuantityChanged { get; set; }

    public int BeforeQuantity { get; set; }

    public int AfterQuantity { get; set; }

    public int CreatedBy { get; set; }
}
