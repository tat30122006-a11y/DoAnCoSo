namespace DoAnCoSo.Models.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "Hoạt động" : "Bị khóa";
    }
}