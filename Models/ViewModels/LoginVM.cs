using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; } = null!;
    }

    // Dùng để lưu thông tin gọn nhẹ của người dùng vào Session sau khi đăng nhập thành công
    public class UserSessionVM
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}