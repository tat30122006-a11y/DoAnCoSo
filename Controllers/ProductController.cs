using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using DoAnCoSo.Services;
using DoAnCoSo.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly QuanLyKhoContext _context;

        public ProductController(IProductService productService, QuanLyKhoContext context)
        {
            _productService = productService;
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllActiveProductsAsync();
            return View(products);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = new Product
            {
                Sku = model.SKU,
                Barcode = model.Barcode,
                Name = model.Name,
                Category = model.Category,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã thêm sản phẩm mới: {product.Name}");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Sku = model.SKU;
            product.Barcode = model.Barcode;
            product.Name = model.Name;
            product.Category = model.Category;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            TempData.SetSuccessMessage($"Đã cập nhật sản phẩm: {product.Name}");
            return RedirectToAction(nameof(Index));
        }
    }
}