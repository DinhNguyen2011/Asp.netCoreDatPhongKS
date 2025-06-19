using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class HoaDonDichVuController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public HoaDonDichVuController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: HoaDonDichVu/Index
        public IActionResult Index(int? maDonHangDv, string trangThaiThanhToan)
        {
            var hoaDonDichVus = _context.HoaDonDichVus
                .Include(hd => hd.MaDonHangDvNavigation)
                .Include(hd => hd.MaHoaDonTongNavigation)
                .AsQueryable();

            if (maDonHangDv.HasValue)
            {
                hoaDonDichVus = hoaDonDichVus.Where(hd => hd.MaDonHangDv == maDonHangDv);
            }

            if (!string.IsNullOrEmpty(trangThaiThanhToan))
            {
                hoaDonDichVus = hoaDonDichVus.Where(hd => hd.TrangThaiThanhToan.Contains(trangThaiThanhToan));
            }

            ViewBag.MaDonHangDv = maDonHangDv;
            ViewBag.TrangThaiThanhToan = trangThaiThanhToan;

            return View(hoaDonDichVus.ToList());
        }

        // GET: HoaDonDichVu/Create
        public IActionResult Create()
        {
            ViewBag.MaDonHangDv = new SelectList(_context.DonHangDichVus, "MaDonHangDv", "MaDonHangDv");
            ViewBag.MaHoaDonTong = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon");
            return View();
        }

        // POST: HoaDonDichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDonDichVu hoaDonDichVu)
        {
            // Ensure MaHoaDonTong is null if no selection is made
            if (string.IsNullOrEmpty(Request.Form["MaHoaDonTong"]))
            {
                hoaDonDichVu.MaHoaDonTong = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(hoaDonDichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaDonHangDv = new SelectList(_context.DonHangDichVus, "MaDonHangDv", "MaDonHangDv", hoaDonDichVu.MaDonHangDv);
            ViewBag.MaHoaDonTong = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon", hoaDonDichVu.MaHoaDonTong);
            return View(hoaDonDichVu);
        }

        // GET: HoaDonDichVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDichVu = await _context.HoaDonDichVus.FindAsync(id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }

            ViewBag.MaDonHangDv = new SelectList(_context.DonHangDichVus, "MaDonHangDv", "MaDonHangDv", hoaDonDichVu.MaDonHangDv);
            ViewBag.MaHoaDonTong = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon", hoaDonDichVu.MaHoaDonTong);
            return View(hoaDonDichVu);
        }

        // POST: HoaDonDichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HoaDonDichVu hoaDonDichVu)
        {
            if (id != hoaDonDichVu.Id)
            {
                return NotFound();
            }

            // Ensure MaHoaDonTong is null if no selection is made
            if (string.IsNullOrEmpty(Request.Form["MaHoaDonTong"]))
            {
                hoaDonDichVu.MaHoaDonTong = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonDichVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.HoaDonDichVus.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaDonHangDv = new SelectList(_context.DonHangDichVus, "MaDonHangDv", "MaDonHangDv", hoaDonDichVu.MaDonHangDv);
            ViewBag.MaHoaDonTong = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon", hoaDonDichVu.MaHoaDonTong);
            return View(hoaDonDichVu);
        }

        // GET: HoaDonDichVu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDichVu = await _context.HoaDonDichVus
                .Include(hd => hd.MaDonHangDvNavigation)
                .Include(hd => hd.MaHoaDonTongNavigation)
                .FirstOrDefaultAsync(hd => hd.Id == id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }

            return View(hoaDonDichVu);
        }

        // POST: HoaDonDichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDonDichVu = await _context.HoaDonDichVus.FindAsync(id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }

            _context.HoaDonDichVus.Remove(hoaDonDichVu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}