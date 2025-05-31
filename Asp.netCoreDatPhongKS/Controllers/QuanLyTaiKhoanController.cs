using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class QuanLyTaiKhoanController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public QuanLyTaiKhoanController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // Quản lý: Danh sách tất cả tài khoản
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Index()
        {
            var taiKhoans = await _context.TaiKhoans
                .Include(t => t.VaiTro)
                .Include(t => t.NhanViens)
                .Where(t => t.VaiTroId == 2 || t.VaiTroId == 3)
                .ToListAsync();
            return View("~/Views/QuanLyTaiKhoan/Index.cshtml", taiKhoans);
        }

        // Quản lý: Thêm tài khoản mới
        [AuthorizePermission("ManageTaiKhoan")]
        public IActionResult Create()
        {
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

            var existingAccount = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == model.TaiKhoan.Email);
            if (existingAccount != null)
            {
                TempData["Error"] = "Email đã được sử dụng.";
                return View("~/Views/QuanLyTaiKhoan/Create.cshtml", model);
            }

            var newTaiKhoan = new TaiKhoan
            {
                Email = model.TaiKhoan.Email,
                MatKhau = model.MatKhau,
                Hoten = model.TaiKhoan.Hoten,
                TrangThai = model.TaiKhoan.TrangThai ?? true,
                VaiTroId = model.VaiTroId,
                HinhAnh = model.TaiKhoan.HinhAnh
            };

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
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Thêm tài khoản thành công.";
            return RedirectToAction("Index");
        }

        // Quản lý: Sửa tài khoản
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Edit(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanViens)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (taiKhoan == null)
                return NotFound();

            var model = new TaiKhoanViewModel
            {
                TaiKhoan = taiKhoan,
                NhanVien = taiKhoan.NhanViens.FirstOrDefault() ?? new NhanVien(),
                VaiTroId = taiKhoan.VaiTroId ?? 3 // Mặc định là Khách hàng nếu VaiTroId null
            };
            return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Edit(int taiKhoanId, TaiKhoanViewModel model, string? newMatKhau)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
            }

            if (model.VaiTroId == 2 && (model.NhanVien == null || string.IsNullOrEmpty(model.NhanVien.Cccd) || string.IsNullOrEmpty(model.NhanVien.SoDienThoai)))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin nhân viên.";
                return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
            }

            var existingTaiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanViens)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (existingTaiKhoan == null)
                return NotFound();

            var emailConflict = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == model.TaiKhoan.Email && t.TaiKhoanId != taiKhoanId);
            if (emailConflict != null)
            {
                TempData["Error"] = "Email đã được sử dụng.";
                return View("~/Views/QuanLyTaiKhoan/Edit.cshtml", model);
            }

            existingTaiKhoan.Email = model.TaiKhoan.Email;
            existingTaiKhoan.Hoten = model.TaiKhoan.Hoten;
            existingTaiKhoan.TrangThai = model.TaiKhoan.TrangThai ?? true;
            existingTaiKhoan.HinhAnh = model.TaiKhoan.HinhAnh;
            if (!string.IsNullOrEmpty(newMatKhau))
                existingTaiKhoan.MatKhau = newMatKhau;

            if (existingTaiKhoan.VaiTroId == 2)
            {
                var existingNhanVien = existingTaiKhoan.NhanViens.FirstOrDefault();
                if (existingNhanVien == null)
                {
                    existingNhanVien = new NhanVien { TaiKhoanId = taiKhoanId };
                    _context.NhanViens.Add(existingNhanVien);
                }
                existingNhanVien.HoTen = model.TaiKhoan.Hoten;
                existingNhanVien.Cccd = model.NhanVien!.Cccd;
                existingNhanVien.DiaChi = model.NhanVien!.DiaChi;
                existingNhanVien.SoDienThoai = model.NhanVien!.SoDienThoai;
                existingNhanVien.Email = model.TaiKhoan.Email;
                existingNhanVien.HinhAnh = model.NhanVien!.HinhAnh;
            }

            await _context.SaveChangesAsync();

            var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (sessionTaiKhoanId == taiKhoanId.ToString())
            {
                HttpContext.Session.SetString("Hoten", model.TaiKhoan.Hoten);
                HttpContext.Session.SetString("Email", model.TaiKhoan.Email);
            }

            TempData["Success"] = "Sửa tài khoản thành công.";
            return RedirectToAction("Index");
        }

        // Quản lý: Xóa tài khoản
        [AuthorizePermission("ManageTaiKhoan")]
        public async Task<IActionResult> Delete(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanViens)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (taiKhoan == null)
                return NotFound();

            var model = new TaiKhoanViewModel
            {
                TaiKhoan = taiKhoan,
                NhanVien = taiKhoan.NhanViens.FirstOrDefault() ?? new NhanVien(),
                VaiTroId = taiKhoan.VaiTroId ?? 3 // Mặc định là Khách hàng nếu VaiTroId null
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
                .Include(t => t.QuyenTaiKhoans)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && (t.VaiTroId == 2 || t.VaiTroId == 3));
            if (taiKhoan == null)
                return NotFound();

            _context.QuyenTaiKhoans.RemoveRange(taiKhoan.QuyenTaiKhoans);
            _context.NhanViens.RemoveRange(taiKhoan.NhanViens);
            _context.TaiKhoans.Remove(taiKhoan);

            await _context.SaveChangesAsync();

            var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (sessionTaiKhoanId == taiKhoanId.ToString())
                HttpContext.Session.Clear();

            TempData["Success"] = "Xóa tài khoản thành công.";
            return RedirectToAction("Index");
        }

        // Nhân viên: Quản lý tài khoản khách hàng
        [AuthorizePermission("ViewCus", "ManageKhachHang")]
        public async Task<IActionResult> ManageKhachHang()
        {
            var khachHangs = await _context.TaiKhoans
                .Include(t => t.VaiTro)
                .Where(t => t.VaiTroId == 3)
                .ToListAsync();
            return View("~/Views/QuanLyTaiKhoan/ManageKhachHang.cshtml", khachHangs);
        }

        // Nhân viên: Thêm tài khoản khách hàng
        [AuthorizePermission("CreateCus", "ManageKhachHang")]
        public IActionResult CreateKhachHang()
        {
            return View("~/Views/QuanLyTaiKhoan/CreateKhachHang.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("CreateCus", "ManageKhachHang")]
        public async Task<IActionResult> CreateKhachHang(TaiKhoan taiKhoan, string matKhau)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(taiKhoan.Email) || string.IsNullOrEmpty(matKhau) || string.IsNullOrEmpty(taiKhoan.Hoten))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View("~/Views/QuanLyTaiKhoan/CreateKhachHang.cshtml", taiKhoan);
            }

            var existingAccount = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == taiKhoan.Email);
            if (existingAccount != null)
            {
                TempData["Error"] = "Email đã được sử dụng.";
                return View("~/Views/QuanLyTaiKhoan/CreateKhachHang.cshtml", taiKhoan);
            }

            var newTaiKhoan = new TaiKhoan
            {
                Email = taiKhoan.Email,
                MatKhau = matKhau,
                Hoten = taiKhoan.Hoten,
                TrangThai = taiKhoan.TrangThai ?? true,
                VaiTroId = 3,
                HinhAnh = taiKhoan.HinhAnh
            };

            _context.TaiKhoans.Add(newTaiKhoan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm tài khoản khách hàng thành công.";
            return RedirectToAction("ManageKhachHang");
        }

        // Nhân viên: Sửa tài khoản khách hàng
        [AuthorizePermission("EditCus", "ManageKhachHang")]
        public async Task<IActionResult> EditKhachHang(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && t.VaiTroId == 3);
            if (taiKhoan == null)
                return NotFound();

            return View("~/Views/QuanLyTaiKhoan/EditKhachHang.cshtml", taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("EditCus", "ManageKhachHang")]
        public async Task<IActionResult> EditKhachHang(int taiKhoanId, TaiKhoan taiKhoan, string? newMatKhau)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(taiKhoan.Email) || string.IsNullOrEmpty(taiKhoan.Hoten))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View("~/Views/QuanLyTaiKhoan/EditKhachHang.cshtml", taiKhoan);
            }

            var existingTaiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && t.VaiTroId == 3);
            if (existingTaiKhoan == null)
                return NotFound();

            var emailConflict = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == taiKhoan.Email && t.TaiKhoanId != taiKhoanId);
            if (emailConflict != null)
            {
                TempData["Error"] = "Email đã được sử dụng.";
                return View("~/Views/QuanLyTaiKhoan/EditKhachHang.cshtml", taiKhoan);
            }

            existingTaiKhoan.Email = taiKhoan.Email;
            existingTaiKhoan.Hoten = taiKhoan.Hoten;
            existingTaiKhoan.TrangThai = taiKhoan.TrangThai ?? true;
            existingTaiKhoan.HinhAnh = taiKhoan.HinhAnh;
            if (!string.IsNullOrEmpty(newMatKhau))
                existingTaiKhoan.MatKhau = newMatKhau;

            await _context.SaveChangesAsync();

            var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (sessionTaiKhoanId == taiKhoanId.ToString())
            {
                HttpContext.Session.SetString("Hoten", taiKhoan.Hoten);
                HttpContext.Session.SetString("Email", taiKhoan.Email);
            }

            TempData["Success"] = "Sửa tài khoản khách hàng thành công.";
            return RedirectToAction("ManageKhachHang");
        }

        // Nhân viên: Xóa tài khoản khách hàng
        [AuthorizePermission("DeleteCus", "ManageKhachHang")]
        public async Task<IActionResult> DeleteKhachHang(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && t.VaiTroId == 3);
            if (taiKhoan == null)
                return NotFound();

            return View("~/Views/QuanLyTaiKhoan/DeleteKhachHang.cshtml", taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("DeleteCus", "ManageKhachHang")]
        public async Task<IActionResult> DeleteKhachHangConfirmed(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.QuyenTaiKhoans)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && t.VaiTroId == 3);
            if (taiKhoan == null)
                return NotFound();

            _context.QuyenTaiKhoans.RemoveRange(taiKhoan.QuyenTaiKhoans);
            _context.TaiKhoans.Remove(taiKhoan);

            await _context.SaveChangesAsync();

            var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (sessionTaiKhoanId == taiKhoanId.ToString())
                HttpContext.Session.Clear();

            TempData["Success"] = "Xóa tài khoản khách hàng thành công.";
            return RedirectToAction("ManageKhachHang");
        }
    }
}