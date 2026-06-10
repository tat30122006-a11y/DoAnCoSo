using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null || userSession.Role != "quản lý")
                return RedirectToAction("Login", "Auth");

            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSessionVM>("UserSession");
            if (userSession == null || userSession.Role != "quản lý")
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model, string rawPassword)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.CreateUserAsync(model, rawPassword);
            if (result.IsSuccess)
            {
                TempData.SetSuccessMessage(result.Message);
                return RedirectToAction(nameof(Index));
            }

            TempData.SetErrorMessage(result.Message);
            return View(model);
        }
    }
}