using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class DanhGiumController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public DanhGiumController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: DanhGium/Index
        public IActionResult Index(int? PhongId, int? dichVuId, int? taiKhoanId, string noiDung)
        {
            var danhGiums = _context.DanhGia
                .Include(dg => dg.Phong)
                .Include(dg => dg.DichVu)
                .Include(dg => dg.TaiKhoan)
                .AsQueryable();

            if (PhongId.HasValue)
            {
                danhGiums = danhGiums.Where(dg => dg.PhongId == PhongId);
            }

            if (dichVuId.HasValue)
            {
                danhGiums = danhGiums.Where(dg => dg.DichVuId == dichVuId);
            }

            if (taiKhoanId.HasValue)
            {
                danhGiums = danhGiums.Where(dg => dg.TaiKhoanId == taiKhoanId);
            }

            if (!string.IsNullOrEmpty(noiDung))
            {
                danhGiums = danhGiums.Where(dg => dg.NoiDung != null && dg.NoiDung.Contains(noiDung));
            }

            ViewBag.PhongId = new SelectList(_context.Phongs, "PhongId", "SoPhong", PhongId);
            ViewBag.DichVuId = new SelectList(_context.DichVus, "DichVuId", "TenDichVu", dichVuId);
            ViewBag.TaiKhoanId = new SelectList(_context.TaiKhoans, "TaiKhoanId", "Hoten", taiKhoanId);
            ViewBag.NoiDung = noiDung;

            return View(danhGiums.ToList());
        }

        // GET: DanhGium/Create
        public IActionResult Create()
        {
            ViewBag.PhongId = new SelectList(_context.Phongs, "PhongId", "SoPhong");
            ViewBag.DichVuId = new SelectList(_context.DichVus, "DichVuId", "TenDichVu");
            ViewBag.TaiKhoanId = new SelectList(_context.TaiKhoans, "TaiKhoanId", "Hoten");
            return View();
        }

        // POST: DanhGium/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanhGium danhGium)
        {
            // Ensure PhongId and DichVuId are null if no selection is made
            if (string.IsNullOrEmpty(Request.Form["PhongId"]))
            {
                danhGium.PhongId = null;
            }
            if (string.IsNullOrEmpty(Request.Form["DichVuId"]))
            {
                danhGium.DichVuId = null;
            }

            // Validate Diem (1-5)
            if (danhGium.Diem.HasValue && (danhGium.Diem < 1 || danhGium.Diem > 5))
            {
                ModelState.AddModelError("Diem", "Điểm đánh giá phải từ 1 đến 5.");
            }

            if (ModelState.IsValid)
            {
                danhGium.NgayDanhGia = DateTime.Now;
                _context.Add(danhGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PhongId = new SelectList(_context.Phongs, "PhongId", "SoPhong", danhGium.PhongId);
            ViewBag.DichVuId = new SelectList(_context.DichVus, "DichVuId", "TenDichVu", danhGium.DichVuId);
            ViewBag.TaiKhoanId = new SelectList(_context.TaiKhoans, "TaiKhoanId", "Hoten", danhGium.TaiKhoanId);
            return View(danhGium);
        }

        // GET: DanhGium/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium == null)
            {
                return NotFound();
            }

            ViewBag.PhongId = new SelectList(_context.Phongs, "PhongId", "SoPhong", danhGium.PhongId);
            ViewBag.DichVuId = new SelectList(_context.DichVus, "DichVuId", "TenDichVu", danhGium.DichVuId);
            ViewBag.TaiKhoanId = new SelectList(_context.TaiKhoans, "TaiKhoanId", "Hoten", danhGium.TaiKhoanId);
            return View(danhGium);
        }

        // POST: DanhGium/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DanhGium danhGium)
        {
            if (id != danhGium.DanhGiaId)
            {
                return NotFound();
            }

            var existingDanhGium = await _context.DanhGia
                .AsNoTracking()
                .FirstOrDefaultAsync(dg => dg.DanhGiaId == id);
            if (existingDanhGium == null)
            {
                return NotFound();
            }

            // Ensure PhongId and DichVuId are null if no selection is made
            if (string.IsNullOrEmpty(Request.Form["PhongId"]))
            {
                danhGium.PhongId = null;
            }
            if (string.IsNullOrEmpty(Request.Form["DichVuId"]))
            {
                danhGium.DichVuId = null;
            }

            // Validate Diem (1-5)
            if (danhGium.Diem.HasValue && (danhGium.Diem < 1 || danhGium.Diem > 5))
            {
                ModelState.AddModelError("Diem", "Điểm đánh giá phải từ 1 đến 5.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve NgayDanhGia
                    danhGium.NgayDanhGia = existingDanhGium.NgayDanhGia;

                    _context.Update(danhGium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DanhGia.Any(e => e.DanhGiaId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PhongId = new SelectList(_context.Phongs, "PhongId", "SoPhong", danhGium.PhongId);
            ViewBag.DichVuId = new SelectList(_context.DichVus, "DichVuId", "TenDichVu", danhGium.DichVuId);
            ViewBag.TaiKhoanId = new SelectList(_context.TaiKhoans, "TaiKhoanId", "Hoten", danhGium.TaiKhoanId);
            return View(danhGium);
        }

        // GET: DanhGium/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(dg => dg.Phong)
                .Include(dg => dg.DichVu)
                .Include(dg => dg.TaiKhoan)
                .FirstOrDefaultAsync(dg => dg.DanhGiaId == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // POST: DanhGium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium == null)
            {
                return NotFound();
            }

            _context.DanhGia.Remove(danhGium);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}