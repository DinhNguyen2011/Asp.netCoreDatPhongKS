using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class LoaiPhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/loaiphong");
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string _defaultImage = "/images/loaiphong/default.png";

        public LoaiPhongController(HotelPlaceVipContext context)
        {
            _context = context;
            // Tạo thư mục images/loaiphong nếu chưa có
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }
        }

        // GET: LoaiPhong/Index (Public view)
        public IActionResult Index(string loai, int? minGia, int? maxGia, int? soNguoi)
        {
            var danhSach = _context.LoaiPhongs.AsQueryable();

            if (!string.IsNullOrEmpty(loai))
                danhSach = danhSach.Where(lp => lp.TenLoai.Contains(loai));

            if (minGia.HasValue)
                danhSach = danhSach.Where(lp => lp.GiaCoBan >= minGia);

            if (maxGia.HasValue)
                danhSach = danhSach.Where(lp => lp.GiaCoBan <= maxGia);

            if (soNguoi.HasValue)
                danhSach = danhSach.Where(lp => lp.SoluongNguoi == soNguoi);

            ViewBag.Loai = loai;
            ViewBag.MinGia = minGia;
            ViewBag.MaxGia = maxGia;
            ViewBag.SoNguoi = soNguoi;

            return View(danhSach.ToList());
        }

        // GET: LoaiPhong/IndexChoAdmin (Admin view)
        public IActionResult IndexChoAdmin(string loai, int? minGia, int? maxGia, int? soNguoi)
        {
            var danhSach = _context.LoaiPhongs.AsQueryable();

            if (!string.IsNullOrEmpty(loai))
                danhSach = danhSach.Where(lp => lp.TenLoai.Contains(loai));

            if (minGia.HasValue)
                danhSach = danhSach.Where(lp => lp.GiaCoBan >= minGia);

            if (maxGia.HasValue)
                danhSach = danhSach.Where(lp => lp.GiaCoBan <= maxGia);

            if (soNguoi.HasValue)
                danhSach = danhSach.Where(lp => lp.SoluongNguoi == soNguoi);

            ViewBag.Loai = loai;
            ViewBag.MinGia = minGia;
            ViewBag.MaxGia = maxGia;
            ViewBag.SoNguoi = soNguoi;

            return View(danhSach.ToList());
        }

        // GET: LoaiPhong/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiPhong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiPhong loaiPhong, IFormFile? anhDemoFile)
        {
            // Xử lý file hình ảnh
            if (anhDemoFile != null && anhDemoFile.Length > 0)
            {
                if (!ValidateAndSaveImage(anhDemoFile, loaiPhong.TenLoai, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("anhDemoFile", errorMessage);
                    return View(loaiPhong);
                }
                loaiPhong.AnhDemo = imagePath;
            }
            else
            {
                loaiPhong.AnhDemo = _defaultImage;
            }

            if (ModelState.IsValid)
            {
                loaiPhong.NgayTao = DateTime.Now;
                _context.Add(loaiPhong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexChoAdmin));
            }

            return View(loaiPhong);
        }

        // GET: LoaiPhong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiPhong = await _context.LoaiPhongs.FindAsync(id);
            if (loaiPhong == null)
            {
                return NotFound();
            }

            return View(loaiPhong);
        }

        // POST: LoaiPhong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiPhong loaiPhongInput, IFormFile? anhDemoFile)
        {
            if (id != loaiPhongInput.LoaiPhongId)
            {
                return NotFound();
            }

            var existingLoaiPhong = await _context.LoaiPhongs.AsNoTracking().FirstOrDefaultAsync(lp => lp.LoaiPhongId == id);
            if (existingLoaiPhong == null)
            {
                return NotFound();
            }

            // Xử lý file hình ảnh
            if (anhDemoFile != null && anhDemoFile.Length > 0)
            {
                if (!ValidateAndSaveImage(anhDemoFile, loaiPhongInput.TenLoai, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("anhDemoFile", errorMessage);
                    return View(loaiPhongInput);
                }
                loaiPhongInput.AnhDemo = imagePath;
            }
            else
            {
                loaiPhongInput.AnhDemo = existingLoaiPhong.AnhDemo;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Giữ nguyên NgayTao
                    loaiPhongInput.NgayTao = existingLoaiPhong.NgayTao;

                    _context.Update(loaiPhongInput);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiPhongs.Any(e => e.LoaiPhongId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(IndexChoAdmin));
            }

            return View(loaiPhongInput);
        }

        // GET: LoaiPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiPhong = await _context.LoaiPhongs
                .Include(lp => lp.Phongs)
                .FirstOrDefaultAsync(lp => lp.LoaiPhongId == id);
            if (loaiPhong == null)
            {
                return NotFound();
            }

            // Kiểm tra khóa ngoại
            if (loaiPhong.Phongs.Any())
            {
                ViewBag.ErrorMessage = "Không thể xóa loại phòng này vì có phòng đang sử dụng nó.";
            }

            return View(loaiPhong);
        }

        // POST: LoaiPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiPhong = await _context.LoaiPhongs
                .Include(lp => lp.Phongs)
                .FirstOrDefaultAsync(lp => lp.LoaiPhongId == id);
            if (loaiPhong == null)
            {
                return NotFound();
            }

            // Kiểm tra khóa ngoại
            if (loaiPhong.Phongs.Any())
            {
                ViewBag.ErrorMessage = "Không thể xóa loại phòng này vì có phòng đang sử dụng nó.";
                return View(loaiPhong);
            }

            _context.LoaiPhongs.Remove(loaiPhong);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexChoAdmin));
        }

        private bool ValidateAndSaveImage(IFormFile file, string tenLoai, out string imagePath, out string errorMessage)
        {
            imagePath = _defaultImage;
            errorMessage = string.Empty;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                errorMessage = "Chỉ chấp nhận các định dạng: .jpg, .jpeg, .png, .gif";
                return false;
            }

            var fileName = $"{tenLoai}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            imagePath = $"/images/loaiphong/{fileName}";
            return true;
        }
    }
}