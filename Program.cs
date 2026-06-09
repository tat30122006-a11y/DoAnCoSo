// Thay thế "DoAnCoSo" bằng Namespace chuẩn của dự án nếu bạn đặt tên khác
using DoAnCoSo.Models;
using DoAnCoSo.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// ĐĂNG KÝ CÁC DỊCH VỤ (SERVICES CONTAINER)
// =================================================================

// 1. Thêm dịch vụ cho kiến trúc MVC (Controllers với Views)
builder.Services.AddControllersWithViews();

// 2. Đăng ký kết nối SQL Server thông qua Entity Framework Core
builder.Services.AddDbContext<QuanLyKhoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CHÈN THÊM VÀO ĐÂY: Đăng ký dịch vụ Session
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tự hủy sau 30 phút không thao tác
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ... Phần đăng ký AddScoped Scoped Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// =================================================================
// CẤU HÌNH ĐƯỜNG ỐNG XỬ LÝ REQUEST (MIDDLEWARE PIPELINE)
// =================================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Kích hoạt Middleware Session (Phải đặt TRƯỚC UseAuthorization)
app.UseSession();

app.UseAuthorization();

// Định tuyến mặc định cho các Controller của MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();