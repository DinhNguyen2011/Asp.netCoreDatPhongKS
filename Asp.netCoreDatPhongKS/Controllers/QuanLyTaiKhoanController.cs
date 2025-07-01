using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]
    public class QuanLyTaiKhoanController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public QuanLyTaiKhoanController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Index(int? vaiTroId)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var query = _context.TaiKhoans
                .Include(t => t.VaiTro)
                .Include(t => t.NhanViens)
                .Include(t => t.KhachHangs)
                .Where(t => t.VaiTroId == 2 || t.VaiTroId == 3);

            if (vaiTroId.HasValue)
            {
                query = query.Where(t => t.VaiTroId == vaiTroId.Value);
            }

            var taiKhoans = await query.ToListAsync();
            ViewBag.VaiTroId = vaiTroId;
            return View("~/Views/QuanLyTaiKhoan/Index.cshtml", taiKhoans);
        }

        [AuthorizePermission("ManageTaiKhoan")]
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View("~/Views/QuanLyTaiKhoan/Create.cshtml", new TaiKhoanViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Create(TaiKhoanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View("~/Views/QuanLyTaiKhoan/Create.cshtml", model);
            }

            if (model.VaiTroId == 2 && (model.NhanVien == null || string.IsNullOrEmpty(model.NhanVien.Cccd) || string.IsNullOrEmpty(model.NhanVien.SoDienThoai)))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin nhân viên.";
                return View("~/Views/QuanLyTaiKhoan/Create.cshtml", model);
            }

            if (model.VaiTroId == 3 && (model.NhanVien == null || string.IsNullOrEmpty(model.NhanVien.Cccd) || string.IsNullOrEmpty(model.NhanVien.SoDienThoai) || string.IsNullOrEmpty(model.NhanVien.DiaChi)))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin khách hàng.";
                return View("~/Views/QuanLyTaiKhoan/Create.cshtml", model);
            }

            var existingAccount = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == model.TaiKhoan.Email);
            if (existingAccount != null)
            {
                TempData["Error"] = "Email đã được sử dụng.";
                return View("~/Views/QuanLyTaiKhoan/Create.cshtml", model);
            }

            var newTaiKhoan = new TaiKhoan
            {
                Email = model.TaiKhoan.Email,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                Hoten = model.TaiKhoan.Hoten,
                TrangThai = model.TaiKhoan.TrangThai ?? true,
                VaiTroId = model.VaiTroId,
                HinhAnh = model.TaiKhoan.HinhAnh,
                NgayTao = DateTime.Now
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.TaiKhoans.Add(newTaiKhoan);
                await _context.SaveChangesAsync();

                if (model.VaiTroId == 2)
                {
                    var newNhanVien = new NhanVien
                    {
                        TaiKhoanId = newTaiKhoan.TaiKhoanId,
                        HoTen = model.TaiKhoan.Hoten,
                        Cccd = model.NhanVien!.Cccd,
                        DiaChi = model.NhanVien!.DiaChi,
                        SoDienThoai = model.NhanVien!.SoDienThoai,
                        Email = model.TaiKhoan.Email,
                        HinhAnh = model.NhanVien!.HinhAnh
                    };
                    _context.NhanViens.Add(newNhanVien);
                }
                else if (model.VaiTroId == 3)
                {
                    var newKhachHang = new KhachHang
                    {
                        TaiKhoanId = newTaiKhoan.TaiKhoanId,
                        HoTen = model.TaiKhoan.Hoten,
                        Cccd = model.NhanVien!.Cccd,
                        DiaChi = model.NhanVien!.DiaChi,
                        SoDienThoai = model.NhanVien!.SoDienThoai,
                        Email = model.TaiKhoan.Email,
                        NgayTao = DateTime.Now
                    };
                    _context.KhachHangs.Add(newKhachHang);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Thêm tài khoản thành công.";
                return RedirectToAction("Index");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra khi thêm tài khoản.";
                return View("~/Views/QuanLyTaiKhoan/Create.cshtml", model);
            }
        }

        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Edit(int taiKhoanId)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (taiKhoan == null)
                return NotFound();

            // Đảm bảo TrangThai có giá trị mặc định
            taiKhoan.TrangThai ??= true;

            return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Edit(int taiKhoanId, TaiKhoan model, string? newMatKhau)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Hoten) || model.TrangThai == null)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ Email, Họ tên và Trạng thái.";
                return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
            }

            var existingTaiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (existingTaiKhoan == null)
                return NotFound();

            var emailConflict = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == model.Email && t.TaiKhoanId != taiKhoanId);
            if (emailConflict != null)
            {
                TempData["Error"] = "Email đã được sử dụng.";
                return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingTaiKhoan.Email = model.Email;
                existingTaiKhoan.Hoten = model.Hoten;
                existingTaiKhoan.TrangThai = model.TrangThai ?? true;
                existingTaiKhoan.HinhAnh = model.HinhAnh;
                if (!string.IsNullOrEmpty(newMatKhau))
                    existingTaiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(newMatKhau);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
                if (sessionTaiKhoanId == taiKhoanId.ToString())
                {
                    HttpContext.Session.SetString("Hoten", model.Hoten);
                    HttpContext.Session.SetString("Email", model.Email);
                }

                TempData["Success"] = "Sửa tài khoản thành công.";
                return RedirectToAction("Index");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra khi sửa tài khoản.";
                return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
            }
        }

        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Delete(int taiKhoanId)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanViens)
                .Include(t => t.KhachHangs)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (taiKhoan == null)
                return NotFound();

            var model = new TaiKhoanViewModel
            {
                TaiKhoan = taiKhoan,
              //  NhanVien = taiKhoan.NhanViens.FirstOrDefault() ?? taiKhoan.KhachHangs.FirstOrDefault() ?? new NhanVien(),
                VaiTroId = taiKhoan.VaiTroId ?? 3
            };
            return View("~/Views/QuanLyTaiKhoan/Delete.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> DeleteConfirmed(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanViens)
                .Include(t => t.KhachHangs)
                .Include(t => t.QuyenTaiKhoans)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (taiKhoan == null)
                return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.QuyenTaiKhoans.RemoveRange(taiKhoan.QuyenTaiKhoans);
                _context.NhanViens.RemoveRange(taiKhoan.NhanViens);
                _context.KhachHangs.RemoveRange(taiKhoan.KhachHangs);
                _context.TaiKhoans.Remove(taiKhoan);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
                if (sessionTaiKhoanId == taiKhoanId.ToString())
                    HttpContext.Session.Clear();

                TempData["Success"] = "Xóa tài khoản thành công.";
                return RedirectToAction("Index");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra khi xóa tài khoản.";
                return RedirectToAction("Index");
            }
        }
    }
}