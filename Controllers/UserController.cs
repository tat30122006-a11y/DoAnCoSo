using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class UserController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public UserController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // Xem danh sách toàn bộ nhân viên trong hệ thống
        public async Task<IActionResult> Index()
        {
            // Kiểm tra quyền: Chỉ cho phép tài khoản "quản lý" xem trang này
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null || userSession.Role != "quản lý") return RedirectToAction("Login", "Auth");

            var users = await _context.Users.OrderBy(u => u.Id).ToListAsync();
            return View(users);
        }

        // Màn hình Thêm nhân viên mới (GET)
        public IActionResult Create()
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null || userSession.Role != "quản lý") return RedirectToAction("Login", "Auth");
            return View();
        }

        // Xử lý lưu nhân viên mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model, string rawPassword)
        {
            if (string.IsNullOrEmpty(rawPassword))
            {
                ModelState.AddModelError("PasswordHash", "Mật khẩu bắt buộc không được để trống");
                return View(model);
            }

            // Kiểm tra xem Username đã tồn tại dưới DB chưa
            var isExist = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (isExist)
            {
                TempData.SetErrorMessage("Tên đăng nhập này đã có người sử dụng!");
                return View(model);
            }

            try
            {
                // BẮT BUỘC: Sử dụng SecurityHelper để băm mật khẩu trước khi cất xuống DB
                model.PasswordHash = SecurityHelper.ComputeSha256Hash(rawPassword);
                model.IsActive = true;

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                TempData.SetSuccessMessage($"Đã cấp tài khoản thành công cho nhân viên: {model.FullName}");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                TempData.SetErrorMessage("Lỗi tạo tài khoản: " + ex.Message);
                return View(model);
            }
        }
    }
}