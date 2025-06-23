using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BCrypt.Net;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

                    if (!taiKhoan.MatKhau.StartsWith("$2a$") && !taiKhoan.MatKhau.StartsWith("$2b$") && !taiKhoan.MatKhau.StartsWith("$2y$"))
                    {
                        if (taiKhoan.MatKhau == model.MatKhau)
                        {
                            isPasswordValid = true;
                            taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        isPasswordValid = BCrypt.Net.BCrypt.Verify(model.MatKhau, taiKhoan.MatKhau);
                    }

                    if (isPasswordValid)
                    {
                        HttpContext.Session.SetString("TaiKhoanId", taiKhoan.TaiKhoanId.ToString());
                        HttpContext.Session.SetString("Hoten", taiKhoan.Hoten ?? "");
                        HttpContext.Session.SetString("Email", taiKhoan.Email ?? "");
                        HttpContext.Session.SetString("VaiTroId", taiKhoan.VaiTroId?.ToString() ?? "");


                        var quyen = taiKhoan.QuyenTaiKhoans.Select(tq => tq.Quyen.MaQuyen).ToList();
                        HttpContext.Session.SetString("Quyen", JsonConvert.SerializeObject(quyen));

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
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                HinhAnh = model.HinhAnh,
                Hoten = model.Hoten,
                VaiTroId = 3,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            TempData["RegisterSuccess"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            TempData["OpenLoginForm"] = true;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var taiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (string.IsNullOrEmpty(taiKhoanId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem thông tin tài khoản.";
                return RedirectToAction("Index", "Home");
            }

            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(tk => tk.TaiKhoanId == int.Parse(taiKhoanId) && tk.TrangThai == true);

            if (taiKhoan == null)
            {
                TempData["LoginError"] = "Tài khoản không tồn tại hoặc đã bị khóa.";
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            // Trả về view dựa trên VaiTroId
            if (taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2)
                return View("ProfileAdmin", taiKhoan);
            else
                return View("ProfileUser", taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(TaiKhoan model, IFormFile? hinhAnh)
        {
            var taiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (string.IsNullOrEmpty(taiKhoanId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để chỉnh sửa thông tin.";
                return RedirectToAction("Index", "Home");
            }

            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(tk => tk.TaiKhoanId == int.Parse(taiKhoanId) && tk.TrangThai == true);

            if (taiKhoan == null)
            {
                TempData["LoginError"] = "Tài khoản không tồn tại hoặc đã bị khóa.";
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Hoten))
            {
                TempData["ProfileError"] = "Vui lòng nhập đầy đủ thông tin.";
                return View(taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2 ? "ProfileAdmin" : "ProfileUser", taiKhoan);
            }

            var existingEmail = _context.TaiKhoans
                .FirstOrDefault(tk => tk.Email == model.Email && tk.TaiKhoanId != taiKhoan.TaiKhoanId);
            if (existingEmail != null)
            {
                TempData["ProfileError"] = "Email đã được sử dụng.";
                return View(taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2 ? "ProfileAdmin" : "ProfileUser", taiKhoan);
            }

            // Cập nhật thông tin
            taiKhoan.Email = model.Email;
            taiKhoan.Hoten = model.Hoten;

            // Xử lý upload ảnh
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(hinhAnh.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ProfileError"] = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif).";
                    return View(taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2 ? "ProfileAdmin" : "ProfileUser", taiKhoan);
                }
                if (hinhAnh.Length > 2 * 1024 * 1024)
                {
                    TempData["ProfileError"] = "File ảnh không được lớn hơn 2MB.";
                    return View(taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2 ? "ProfileAdmin" : "ProfileUser", taiKhoan);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }
                taiKhoan.HinhAnh = "/images/" + fileName;
            }

            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            // Cập nhật session
            HttpContext.Session.SetString("Hoten", taiKhoan.Hoten ?? "");
            HttpContext.Session.SetString("Email", taiKhoan.Email ?? "");
            HttpContext.Session.SetString("UserInfo", JsonConvert.SerializeObject(taiKhoan));

            TempData["ProfileSuccess"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var taiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (string.IsNullOrEmpty(taiKhoanId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để đổi mật khẩu.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var taiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (string.IsNullOrEmpty(taiKhoanId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để đổi mật khẩu.";
                return RedirectToAction("Index", "Home");
            }

            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(tk => tk.TaiKhoanId == int.Parse(taiKhoanId) && tk.TrangThai == true);

            if (taiKhoan == null)
            {
                TempData["LoginError"] = "Tài khoản không tồn tại hoặc đã bị khóa.";
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ChangePasswordError"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, taiKhoan.MatKhau);
            if (!isCurrentPasswordValid)
            {
                TempData["ChangePasswordError"] = "Mật khẩu hiện tại không đúng.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["ChangePasswordError"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return View();
            }

            if (newPassword.Length < 6)
            {
                TempData["ChangePasswordError"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return View();
            }

            taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.SaveChanges();

            HttpContext.Session.Clear();
            TempData["ChangePasswordSuccess"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
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