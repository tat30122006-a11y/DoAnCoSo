using DoAnCoSo.Helpers;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu tài khoản đã có session từ trước, tự động đẩy thẳng vào Dashboard, không bắt đăng nhập lại
            if (HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            // Kiểm tra tính hợp lệ của Form (Bỏ trống tài khoản/mật khẩu)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi UserService xuống DB xác thực (Mật khẩu sẽ được tự động băm SHA256 bên trong Service)
            var userSession = await _userService.AuthenticateAsync(model.Username, model.Password);

            if (userSession == null)
            {
                // Sử dụng NotificationHelper để gán thông báo lỗi màu đỏ ra giao diện
                TempData.SetErrorMessage("Tên đăng nhập hoặc mật khẩu không chính xác!");
                return View(model);
            }

            // Đăng nhập thành công -> Dùng SessionHelper đóng gói Object đưa vào Session bộ nhớ
            HttpContext.Session.SetObjectAsJson("UserSession", userSession);

            // Gán thông báo thành công màu xanh
            TempData.SetSuccessMessage($"Chào mừng {userSession.FullName} đăng nhập hệ thống!");

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Auth/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa sạch Session lưu vết người dùng
            HttpContext.Session.Remove("UserSession");
            TempData.SetSuccessMessage("Đã đăng xuất khỏi hệ thống an toàn.");
            return RedirectToAction("Login");
        }
    }
}