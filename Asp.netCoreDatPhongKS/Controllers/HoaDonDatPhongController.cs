using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]
    public class HoaDonDatPhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public HoaDonDatPhongController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: HoaDonDatPhong
        public async Task<IActionResult> Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var hoaDonPdps = await _context.HoaDonPdps
                .Include(h => h.MaHoaDonNavigation)
                .Include(h => h.PhieuDatPhong)
                .ToListAsync();
            return View(hoaDonPdps);
        }

        // GET: HoaDonDatPhong/Create
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            ViewData["MaHoaDon"] = _context.HoaDons.ToList();
            ViewData["PhieuDatPhongId"] = _context.PhieuDatPhongs.ToList();
            return View();
        }

        // POST: HoaDonDatPhong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDonPdp hoaDonPdp)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonPdp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHoaDon"] = _context.HoaDons.ToList();
            ViewData["PhieuDatPhongId"] = _context.PhieuDatPhongs.ToList();
            return View(hoaDonPdp);
        }

        // GET: HoaDonDatPhong/Edit/5
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

            var hoaDonPdp = await _context.HoaDonPdps.FindAsync(id);
            if (hoaDonPdp == null)
            {
                return NotFound();
            }
            ViewData["MaHoaDon"] = _context.HoaDons.ToList();
            ViewData["PhieuDatPhongId"] = _context.PhieuDatPhongs.ToList();
            return View(hoaDonPdp);
        }

        // POST: HoaDonDatPhong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HoaDonPdp hoaDonPdp)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id != hoaDonPdp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonPdp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonPdpExists(hoaDonPdp.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHoaDon"] = _context.HoaDons.ToList();
            ViewData["PhieuDatPhongId"] = _context.PhieuDatPhongs.ToList();
            return View(hoaDonPdp);
        }

        // GET: HoaDonDatPhong/Delete/5
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

            var hoaDonPdp = await _context.HoaDonPdps
                .Include(h => h.MaHoaDonNavigation)
                .Include(h => h.PhieuDatPhong)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hoaDonPdp == null)
            {
                return NotFound();
            }

            return View(hoaDonPdp);
        }

        // POST: HoaDonDatPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var hoaDonPdp = await _context.HoaDonPdps.FindAsync(id);
            if (hoaDonPdp != null)
            {
                // Check constraints
                var relatedRecords = await _context.HoaDonPdps
                    .AnyAsync(h => h.PhieuDatPhongId == hoaDonPdp.PhieuDatPhongId && h.Id != id);
                if (relatedRecords)
                {
                    TempData["ErrorMessage"] = "Không thể xóa hóa đơn này vì có các bản ghi liên quan.";
                    return RedirectToAction(nameof(Index));
                }

                _context.HoaDonPdps.Remove(hoaDonPdp);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonPdpExists(int id)
        {
            return _context.HoaDonPdps.Any(e => e.Id == id);
        }
    }
}