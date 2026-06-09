using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class StockLoss
{
    public int Id { get; set; }

    public int TransferOrderId { get; set; }

    public int ProductId { get; set; }

    public int LossQty { get; set; }

    public string? Reason { get; set; }

    public bool ResolvedStatus { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual TransferOrder TransferOrder { get; set; } = null!;
}
