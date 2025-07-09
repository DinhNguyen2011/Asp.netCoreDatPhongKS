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
    public class ChiTietPDPController : Controller
    {
        private readonly HotelPlaceVipContext _context; 

        public ChiTietPDPController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: ChiTietPDP/Index
        public async Task<IActionResult> Index()
        {
            var chiTietPhieuPhongs = await _context.ChiTietPhieuPhongs
                .Include(c => c.PhieuDatPhong)
                .Include(c => c.Phong)
                .ToListAsync();
            return View(chiTietPhieuPhongs);
        }

        // POST: ChiTietPDP/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var chiTietPhieuPhong = await _context.ChiTietPhieuPhongs.FindAsync(id);
                if (chiTietPhieuPhong == null)
                {
                    TempData["Error"] = "Không tìm thấy chi tiết phiếu phòng.";
                    return RedirectToAction(nameof(Index));
                }

                _context.ChiTietPhieuPhongs.Remove(chiTietPhieuPhong);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa chi tiết phiếu phòng thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}