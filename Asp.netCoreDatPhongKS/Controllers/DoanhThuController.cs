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
    public class DoanhThuController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public DoanhThuController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ThongKeDoanhThu(DateTime? fromDate, DateTime? toDate)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            // Truy vấn tất cả HoaDon
            var query = _context.HoaDons
                .Where(hd => hd.NgayLap.HasValue);

            // Lọc theo khoảng thời gian
            if (fromDate.HasValue)
            {
                query = query.Where(hd => hd.NgayLap.Value.Date >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                query = query.Where(hd => hd.NgayLap.Value.Date <= toDate.Value.Date);
            }

            // Tính tổng theo ngày
            var thongKe = await query
                .GroupBy(hd => hd.NgayLap.Value.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    TongTien = g.Sum(hd => hd.TongTien),
                    TongTienPhong = g.Sum(hd => hd.TongTienPhong ?? 0),
                    TongTienDichVu = g.Sum(hd => hd.TongTienDichVu ?? 0)
                })
                .OrderBy(x => x.Ngay)
                .ToListAsync();

            ViewBag.ThongKe = thongKe;

            return View("Index");
        }
    }
}