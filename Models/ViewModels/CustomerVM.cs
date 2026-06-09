namespace DoAnCoSo.Models.ViewModels
{
    public class CustomerVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "Hoạt động" : "Ngừng giao dịch";
    }
}