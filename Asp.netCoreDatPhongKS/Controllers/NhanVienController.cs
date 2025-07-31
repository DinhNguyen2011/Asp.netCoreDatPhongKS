using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]
    public class NhanVienController : Controller
    {
        private readonly HotelPlaceVipContext _context;
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/nhanvien");
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string _defaultImage = "/images/nhanvien/default.png";

        public NhanVienController(HotelPlaceVipContext context)
        {
            _context = context;
            // Create images/nhanvien directory if it doesn't exist
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }
        }

        // GET: NhanVien/Index
        public IActionResult Index(string searchString)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var nhanViens = _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                nhanViens = nhanViens.Where(nv => (nv.HoTen != null && nv.HoTen.Contains(searchString)) ||
                                                  (nv.Email != null && nv.Email.Contains(searchString)) ||
                                                  (nv.SoDienThoai != null && nv.SoDienThoai.Contains(searchString)));
            }

            ViewBag.SearchString = searchString;

            return View(nhanViens.ToList());
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVien nhanVien, IFormFile? hinhAnhFile)
        {
            // Handle image upload
            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnhFile, nhanVien.HoTen, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnhFile", errorMessage);
                    return View(nhanVien);
                }
                nhanVien.HinhAnh = imagePath;
            }
            else
            {
                nhanVien.HinhAnh = _defaultImage;
            }

            if (ModelState.IsValid)
            {
                // Create TaiKhoan
                var taiKhoan = new TaiKhoan
                {
                    Email = nhanVien.Email,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword("123"), // Replace with secure password hashing
                    VaiTroId = 2, // Employee role
                    TrangThai = true,
                    Hoten = nhanVien.HoTen,
                    NgayTao = DateTime.Now
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Link NhanVien to TaiKhoan
                nhanVien.TaiKhoanId = taiKhoan.TaiKhoanId;
                nhanVien.NgayTao = DateTime.Now;

                _context.NhanViens.Add(nhanVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(nhanVien);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .FirstOrDefaultAsync(nv => nv.NhanVienId == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhanVien nhanVien, IFormFile? hinhAnhFile)
        {
            if (id != nhanVien.NhanVienId)
            {
                return NotFound();
            }

            var existingNhanVien = await _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .AsNoTracking()
                .FirstOrDefaultAsync(nv => nv.NhanVienId == id);
            if (existingNhanVien == null)
            {
                return NotFound();
            }

            // Handle image upload
            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnhFile, nhanVien.HoTen, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnhFile", errorMessage);
                    return View(nhanVien);
                }
                nhanVien.HinhAnh = imagePath;
            }
            else
            {
                nhanVien.HinhAnh = existingNhanVien.HinhAnh;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve NgayTao
                    nhanVien.NgayTao = existingNhanVien.NgayTao;

                    // Update TaiKhoan Email and Hoten if changed
                    if (nhanVien.TaiKhoanId.HasValue)
                    {
                        var taiKhoan = await _context.TaiKhoans.FindAsync(nhanVien.TaiKhoanId);
                        if (taiKhoan != null)
                        {
                            taiKhoan.Email = nhanVien.Email;
                            taiKhoan.Hoten = nhanVien.HoTen;
                            _context.Update(taiKhoan);
                        }
                    }

                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NhanViens.Any(e => e.NhanVienId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                TempData["SuccessMessage"] = "Sửa thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(nhanVien);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.KhachHangs)
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.DanhGia)
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.LienHeVoiCtois)
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.QuyenTaiKhoans)
                .FirstOrDefaultAsync(nv => nv.NhanVienId == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            // Check foreign key constraints on TaiKhoan
            if (nhanVien.TaiKhoan != null &&
                (nhanVien.TaiKhoan.KhachHangs.Any() || nhanVien.TaiKhoan.DanhGia.Any() ||
                 nhanVien.TaiKhoan.LienHeVoiCtois.Any() || nhanVien.TaiKhoan.QuyenTaiKhoans.Any()))
            {
                ViewBag.ErrorMessage = "Không thể xóa nhân viên này vì tài khoản liên quan có dữ liệu (khách hàng, đánh giá, liên hệ, hoặc quyền).";
            }

            return View(nhanVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var nhanVien = await _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.KhachHangs)
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.DanhGia)
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.LienHeVoiCtois)
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.QuyenTaiKhoans)
                .FirstOrDefaultAsync(nv => nv.NhanVienId == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            // Check foreign key constraints on TaiKhoan
            if (nhanVien.TaiKhoan != null &&
                (nhanVien.TaiKhoan.KhachHangs.Any() || nhanVien.TaiKhoan.DanhGia.Any() ||
                 nhanVien.TaiKhoan.LienHeVoiCtois.Any() || nhanVien.TaiKhoan.QuyenTaiKhoans.Any()))
            {
                ViewBag.ErrorMessage = "Không thể xóa nhân viên này vì tài khoản liên quan có dữ liệu (khách hàng, đánh giá, liên hệ, hoặc quyền).";
                return View(nhanVien);
            }

            // Delete TaiKhoan and NhanVien
            if (nhanVien.TaiKhoan != null)
            {
                _context.TaiKhoans.Remove(nhanVien.TaiKhoan);
            }
            _context.NhanViens.Remove(nhanVien);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValidateAndSaveImage(IFormFile file, string hoTen, out string imagePath, out string errorMessage)
        {
            imagePath = _defaultImage;
            errorMessage = string.Empty;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                errorMessage = "Chỉ chấp nhận các định dạng: .jpg, .jpeg, .png, .gif";
                return false;
            }

            var fileName = $"{hoTen}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            imagePath = $"/images/nhanvien/{fileName}";
            return true;
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(m => m.NhanVienId == id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

    }
}