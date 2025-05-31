using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HotelPlaceVipContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelPlaceVip")));

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
builder.Services.AddScoped<IMoMoService, MoMoService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // CHỈ 1 lần duy nhất ở đây
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
