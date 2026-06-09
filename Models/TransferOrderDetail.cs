using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class TransferOrderDetail
{
    public int Id { get; set; }

    public int TransferOrderId { get; set; }

    public int ProductId { get; set; }

    public int RequestedQty { get; set; }

    public int? ActualQty { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual TransferOrder TransferOrder { get; set; } = null!;
}
