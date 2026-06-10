using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public class UserService : IUserService
    {
        private readonly QuanLyKhoContext _context;

        public UserService(QuanLyKhoContext context)
        {
            _context = context;
        }

        public async Task<UserSessionVM?> AuthenticateAsync(string username, string password)
        {
            // 1. Mã hóa mật khẩu người dùng nhập vào bằng Helper
            string hashedPass = SecurityHelper.ComputeSha256Hash(password);

            // 2. Tìm trong DB xem có tài khoản nào khớp cả Username, Password và đang Active không
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPass && u.IsActive);

            // 3. Nếu không tìm thấy (sai thông tin) thì trả về null
            if (user == null) return null;

            // 4. Nếu đúng, đóng gói thông tin vào ViewModel để trả ra ngoài (Controller sẽ lấy cái này lưu vào Session)
            return new UserSessionVM
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role
            };
        }
        public async Task<List<UserVM>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserVM
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive
                }).ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> CreateUserAsync(User model, string rawPassword)
        {
            var isExist = await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
            if (isExist) return (false, "Tên đăng nhập hoặc Email này đã tồn tại trong hệ thống!");

            try
            {
                model.PasswordHash = SecurityHelper.ComputeSha256Hash(rawPassword);
                model.IsActive = true;
                _context.Users.Add(model);
                await _context.SaveChangesAsync();
                return (true, "Cấp tài khoản nhân viên mới thành công!");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi lưu database: " + ex.Message);
            }
        }
    }
}