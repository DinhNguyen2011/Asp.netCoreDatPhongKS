using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BCrypt.Net; // Thêm thư viện BCrypt

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public TaiKhoanController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                var taiKhoan = _context.TaiKhoans
                    .Include(tk => tk.VaiTro)
                    .Include(tk => tk.QuyenTaiKhoans)
                    .ThenInclude(tq => tq.Quyen)
                    .FirstOrDefault(tk => tk.Email == model.Email && tk.TrangThai == true);

                if (taiKhoan != null)
                {
                    bool isPasswordValid = false;

                    // Kiểm tra nếu mật khẩu chưa mã hóa (plain text)
                    if (!taiKhoan.MatKhau.StartsWith("$2a$") && !taiKhoan.MatKhau.StartsWith("$2b$") && !taiKhoan.MatKhau.StartsWith("$2y$"))
                    {
                        if (taiKhoan.MatKhau == model.MatKhau)
                        {
                            isPasswordValid = true;
                            // Mã hóa lại mật khẩu và cập nhật database
                            taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        // Kiểm tra mật khẩu đã mã hóa
                        isPasswordValid = BCrypt.Net.BCrypt.Verify(model.MatKhau, taiKhoan.MatKhau);
                    }

                    if (isPasswordValid)
                    {
                        HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                        HttpContext.Session.SetString("Hoten", taiKhoan.Hoten ?? "");
                        HttpContext.Session.SetString("Email", taiKhoan.Email ?? "");
                        HttpContext.Session.SetString("VaiTroId", taiKhoan.VaiTroId?.ToString() ?? "");
                        var quyen = taiKhoan.QuyenTaiKhoans.Select(tq => tq.Quyen.MaQuyen).ToList();
                        HttpContext.Session.SetString("Quyen", Newtonsoft.Json.JsonConvert.SerializeObject(quyen));

                        if (taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2)
                            return RedirectToAction("Index", "HomeAdmin");
                        else if (taiKhoan.VaiTroId == 3)
                            return RedirectToAction("Index", "Home");

                        TempData["LoginError"] = "Bạn không có quyền truy cập.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["LoginError"] = "Thông tin đăng nhập hoặc mật khẩu không đúng";
                return RedirectToAction("Index", "Home");
            }

            TempData["LoginError"] = "Vui lòng nhập đầy đủ thông tin.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(TaiKhoan model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.MatKhau) || string.IsNullOrEmpty(model.Hoten))
            {
                TempData["RegisterError"] = "Vui lòng nhập đầy đủ thông tin.";
                TempData["OpenRegisterForm"] = true;
                return RedirectToAction("Index", "Home");
            }

            var existingAccount = _context.TaiKhoans.FirstOrDefault(t => t.Email == model.Email);
            if (existingAccount != null)
            {
                TempData["RegisterError"] = "Email đã được sử dụng.";
                TempData["OpenRegisterForm"] = true;
                return RedirectToAction("Index", "Home");
            }

            TaiKhoan taiKhoan = new TaiKhoan
            {
                Email = model.Email,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau), // Mã hóa mật khẩu
                HinhAnh = model.HinhAnh,
                Hoten = model.Hoten,
                VaiTroId = 3, // Khách hàng
                TrangThai = true
            };

            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            TempData["RegisterSuccess"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            TempData["OpenLoginForm"] = true;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}