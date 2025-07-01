using Asp.netCoreDatPhongKS.Filters;
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
    public class PhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/phong");
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string _defaultImage = "/images/phong/default.png";

        public PhongController(HotelPlaceVipContext context)
        {
            _context = context;
            // Tạo thư mục images/phong nếu chưa có
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }
        }

        // GET: Phong
        public async Task<IActionResult> Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var phongs = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .ToListAsync();
            return View(phongs);
        }

        // GET: Phong/Create
        [HttpGet]
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
            return View();
        }

        // POST: Phong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phong phong, IFormFile? hinhAnhFile, IFormFile? hinhAnh1File, IFormFile? hinhAnh2File, IFormFile? hinhAnh3File)
        {
            // Xử lý các file hình ảnh
            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnhFile, phong.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnhFile", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phong);
                }
                phong.HinhAnh = imagePath;
            }
            else
            {
                phong.HinhAnh = _defaultImage;
            }

            if (hinhAnh1File != null && hinhAnh1File.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnh1File, phong.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnh1File", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phong);
                }
                phong.HinhAnh1 = imagePath;
            }
            else
            {
                phong.HinhAnh1 = _defaultImage;
            }

            if (hinhAnh2File != null && hinhAnh2File.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnh2File, phong.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnh2File", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phong);
                }
                phong.HinhAnh2 = imagePath;
            }
            else
            {
                phong.HinhAnh2 = _defaultImage;
            }

            if (hinhAnh3File != null && hinhAnh3File.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnh3File, phong.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnh3File", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phong);
                }
                phong.HinhAnh3 = imagePath;
            }
            else
            {
                phong.HinhAnh3 = _defaultImage;
            }

            if (ModelState.IsValid)
            {
                phong.NgayTao = DateTime.Now;
                _context.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
            return View(phong);
        }

        // GET: Phong/Edit/5
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

            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null)
            {
                return NotFound();
            }
            ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
            return View(phong);
        }

        // POST: Phong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phong phongInput, IFormFile? hinhAnhFile, IFormFile? hinhAnh1File, IFormFile? hinhAnh2File, IFormFile? hinhAnh3File)
        {
            if (id != phongInput.PhongId)
            {
                return NotFound();
            }

            var existingPhong = await _context.Phongs.AsNoTracking().FirstOrDefaultAsync(p => p.PhongId == id);
            if (existingPhong == null)
            {
                return NotFound();
            }

            // Xử lý các file hình ảnh
            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnhFile, phongInput.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnhFile", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phongInput);
                }
                phongInput.HinhAnh = imagePath;
            }
            else
            {
                phongInput.HinhAnh = existingPhong.HinhAnh;
            }

            if (hinhAnh1File != null && hinhAnh1File.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnh1File, phongInput.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnh1File", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phongInput);
                }
                phongInput.HinhAnh1 = imagePath;
            }
            else
            {
                phongInput.HinhAnh1 = existingPhong.HinhAnh1;
            }

            if (hinhAnh2File != null && hinhAnh2File.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnh2File, phongInput.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnh2File", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phongInput);
                }
                phongInput.HinhAnh2 = imagePath;
            }
            else
            {
                phongInput.HinhAnh2 = existingPhong.HinhAnh2;
            }

            if (hinhAnh3File != null && hinhAnh3File.Length > 0)
            {
                if (!ValidateAndSaveImage(hinhAnh3File, phongInput.SoPhong, out string imagePath, out string errorMessage))
                {
                    ModelState.AddModelError("hinhAnh3File", errorMessage);
                    ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
                    return View(phongInput);
                }
                phongInput.HinhAnh3 = imagePath;
            }
            else
            {
                phongInput.HinhAnh3 = existingPhong.HinhAnh3;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Giữ nguyên NgayTao
                    phongInput.NgayTao = existingPhong.NgayTao;

                    _context.Update(phongInput);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Phongs.Any(e => e.PhongId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.LoaiPhongs = _context.LoaiPhongs.ToList();
            return View(phongInput);
        }

        // GET: Phong/Delete/5
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

            var phong = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(m => m.PhongId == id);
            if (phong == null)
            {
                return NotFound();
            }

            return View(phong);
        }

        // POST: Phong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ValidateAndSaveImage(IFormFile file, string soPhong, out string imagePath, out string errorMessage)
        {
            imagePath = _defaultImage;
            errorMessage = string.Empty;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                errorMessage = "Chỉ chấp nhận các định dạng: .jpg, .jpeg, .png, .gif";
                return false;
            }

            var fileName = $"{soPhong}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            imagePath = $"/images/phong/{fileName}";
            return true;
        }
    }
}