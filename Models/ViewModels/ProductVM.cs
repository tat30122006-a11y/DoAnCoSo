namespace DoAnCoSo.Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string SKU { get; set; } = null!;
        public string? Barcode { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
    }
}