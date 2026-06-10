using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using DoAnCoSo.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly QuanLyKhoContext _context;

        public CustomerController(ICustomerService customerService, QuanLyKhoContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllActiveCustomersAsync();
            return View(customers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = new Customer
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                IsActive = true
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã thêm khách hàng: {customer.Name}");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            var model = new CustomerVM
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                IsActive = customer.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerVM model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.Name = model.Name;
            customer.Phone = model.Phone;
            customer.Email = model.Email;
            customer.Address = model.Address;
            customer.IsActive = model.IsActive;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã cập nhật khách hàng: {customer.Name}");
            return RedirectToAction(nameof(Index));
        }
    }
}