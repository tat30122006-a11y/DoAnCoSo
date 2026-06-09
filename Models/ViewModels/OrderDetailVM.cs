namespace DoAnCoSo.Models.ViewModels
{
    public class OrderDetailVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount => Quantity * Price; // Tự động tính thành tiền
    }
}