using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HotelPlaceVipContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelPlaceVip")));

// Thêm dịch vụ Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/Login"; // Chuyển hướng đến trang login nếu chưa đăng nhập
        options.AccessDeniedPath = "/Home/Index"; // Chuyển hướng nếu không có quyền
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Thời gian hết hạn session
    });

builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IMoMoService, MoMoService>();

// Thêm Services
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<ExchangeService>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
