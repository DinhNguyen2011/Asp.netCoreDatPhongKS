using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            // Lưu trữ ngày để hiển thị trong View
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            // Truy vấn hóa đơn
            var query = _context.HoaDons
                .Where(hd => hd.NgayLap.HasValue);
             // .Where(hd => hd.NgayLap.HasValue && hd.TongTien.HasValue);

            // Lọc theo khoảng thời gian
            if (fromDate.HasValue)
            {
                query = query.Where(hd => hd.NgayLap.Value.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(hd => hd.NgayLap.Value.Date <= toDate.Value.Date);
            }

            // Tạo danh sách thống kê doanh thu theo ngày
            var thongKe = await query
                .GroupBy(hd => hd.NgayLap.Value.Date)
                .Select(g => new DoanhThuViewModel
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(hd => hd.TongTien),
                    DoanhThuDichVu = g.Sum(hd => hd.TongTienDichVu ?? 0),
                    DoanhThuPdp = g.Sum(hd => hd.TongTienPhong ?? 0)
                })
                .OrderBy(x => x.Ngay)
                .ToListAsync();

            return View("Index", thongKe);
        }
    }
}