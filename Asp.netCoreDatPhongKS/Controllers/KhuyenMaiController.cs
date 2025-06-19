using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class KhuyenMaiController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public KhuyenMaiController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: KhuyenMai/Index
        public IActionResult Index(string searchString, bool? trangThai)
        {
            var khuyenMais = _context.KhuyenMais
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                khuyenMais = khuyenMais.Where(km => (km.MaKhuyenMai != null && km.MaKhuyenMai.Contains(searchString)) ||
                                                    (km.MoTa != null && km.MoTa.Contains(searchString)));
            }

            if (trangThai.HasValue)
            {
                khuyenMais = khuyenMais.Where(km => km.TrangThai == trangThai);
            }

            ViewBag.SearchString = searchString;
            ViewBag.TrangThai = trangThai;

            return View(khuyenMais.ToList());
        }

        // GET: KhuyenMai/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhuyenMai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuyenMai khuyenMai)
        {
            // Validate NgayBatDau <= NgayKetThuc
            if (khuyenMai.NgayBatDau.HasValue && khuyenMai.NgayKetThuc.HasValue &&
                khuyenMai.NgayBatDau > khuyenMai.NgayKetThuc)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            // Check if MaKhuyenMai is unique
            if (_context.KhuyenMais.Any(km => km.MaKhuyenMai == khuyenMai.MaKhuyenMai))
            {
                ModelState.AddModelError("MaKhuyenMai", "Mã khuyến mãi đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                khuyenMai.NgayTao = DateTime.Now;
                khuyenMai.TrangThai = khuyenMai.TrangThai ?? true;

                _context.Add(khuyenMai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(khuyenMai);
        }

        // GET: KhuyenMai/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            if (khuyenMai == null)
            {
                return NotFound();
            }

            return View(khuyenMai);
        }

        // POST: KhuyenMai/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhuyenMai khuyenMai)
        {
            if (id != khuyenMai.KhuyenMaiId)
            {
                return NotFound();
            }

            var existingKhuyenMai = await _context.KhuyenMais
                .AsNoTracking()
                .FirstOrDefaultAsync(km => km.KhuyenMaiId == id);
            if (existingKhuyenMai == null)
            {
                return NotFound();
            }

            // Validate NgayBatDau <= NgayKetThuc
            if (khuyenMai.NgayBatDau.HasValue && khuyenMai.NgayKetThuc.HasValue &&
                khuyenMai.NgayBatDau > khuyenMai.NgayKetThuc)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            // Check if MaKhuyenMai is unique (excluding current record)
            if (_context.KhuyenMais.Any(km => km.MaKhuyenMai == khuyenMai.MaKhuyenMai && km.KhuyenMaiId != id))
            {
                ModelState.AddModelError("MaKhuyenMai", "Mã khuyến mãi đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve NgayTao
                    khuyenMai.NgayTao = existingKhuyenMai.NgayTao;

                    _context.Update(khuyenMai);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.KhuyenMais.Any(e => e.KhuyenMaiId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(khuyenMai);
        }

        // GET: KhuyenMai/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenMai = await _context.KhuyenMais
                .Include(km => km.PhieuDatPhongs)
                .FirstOrDefaultAsync(km => km.KhuyenMaiId == id);
            if (khuyenMai == null)
            {
                return NotFound();
            }

            // Check foreign key constraints
            if (khuyenMai.PhieuDatPhongs.Any())
            {
                ViewBag.ErrorMessage = "Không thể xóa khuyến mãi này vì có phiếu đặt phòng liên quan.";
            }

            return View(khuyenMai);
        }

        // POST: KhuyenMai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khuyenMai = await _context.KhuyenMais
                .Include(km => km.PhieuDatPhongs)
                .FirstOrDefaultAsync(km => km.KhuyenMaiId == id);
            if (khuyenMai == null)
            {
                return NotFound();
            }

            // Check foreign key constraints
            if (khuyenMai.PhieuDatPhongs.Any())
            {
                ViewBag.ErrorMessage = "Không thể xóa khuyến mãi này vì có phiếu đặt phòng liên quan.";
                return View(khuyenMai);
            }

            _context.KhuyenMais.Remove(khuyenMai);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}