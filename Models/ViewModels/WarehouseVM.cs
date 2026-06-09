namespace DoAnCoSo.Models.ViewModels
{
    public class WarehouseVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public bool IsActive { get; set; }

        // Trả về chuỗi hiển thị tự động dựa trên trạng thái IsActive
        public string StatusText => IsActive ? "Hoạt động" : "Ngừng hoạt động";
    }
}