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

            var query = _context.HoaDons
                .Join(_context.HoaDonPdps,
                      hd => hd.MaHoaDon,
                      hdp => hdp.MaHoaDon,
                      (hd, hdp) => new { HoaDon = hd, HoaDonPdp = hdp })
                .Join(_context.PhieuDatPhongs,
                      hd_hdp => hd_hdp.HoaDonPdp.PhieuDatPhongId,
                      pdp => pdp.PhieuDatPhongId,
                      (hd_hdp, pdp) => new { hd_hdp.HoaDon, hd_hdp.HoaDonPdp, PhieuDatPhong = pdp })
                .Where(joined => joined.PhieuDatPhong.TinhTrangSuDung != "Đã hủy" && joined.HoaDon.NgayLap.HasValue);

            // Truy vấn dịch vụ từ DonHangDichVu
            var dichVuQuery = _context.ChiTietDonHangDichVus
            
                .Where(dhd => dhd.MaDonHangDvNavigation.NgayDat.HasValue)
                .GroupBy(dhd => dhd.MaDonHangDvNavigation.NgayDat.Value.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    DoanhThuDichVu = g.Sum(dhd => dhd.ThanhTien ?? 0) 
                });

            // Lọc theo khoảng thời gian
            if (fromDate.HasValue)
            {
                query = query.Where(joined => joined.HoaDon.NgayLap.Value.Date >= fromDate.Value.Date);
                dichVuQuery = dichVuQuery.Where(d => d.Ngay >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(joined => joined.HoaDon.NgayLap.Value.Date <= toDate.Value.Date);
                dichVuQuery = dichVuQuery.Where(d => d.Ngay <= toDate.Value.Date);
            }

            // Tạo danh sách thống kê doanh thu theo ngày
            var thongKeHoaDon = await query
                .GroupBy(joined => joined.HoaDon.NgayLap.Value.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(joined => joined.HoaDon.TongTien),
                    DoanhThuPdp = g.Sum(joined => joined.HoaDon.TongTienPhong ?? 0)
                })
                .ToListAsync();

            var thongKeDichVu = await dichVuQuery.ToListAsync();

            // Kết hợp doanh thu hóa đơn và dịch vụ
            var thongKe = from hd in thongKeHoaDon
                          join dv in thongKeDichVu on hd.Ngay equals dv.Ngay into dichVuGroup
                          from dv in dichVuGroup.DefaultIfEmpty()
                          select new DoanhThuViewModel
                          {
                              Ngay = hd.Ngay,
                              DoanhThu = hd.DoanhThu + (dv?.DoanhThuDichVu ?? 0),
                              DoanhThuDichVu = dv?.DoanhThuDichVu ?? 0,
                              DoanhThuPdp = hd.DoanhThuPdp
                          };

            var result = thongKe.OrderBy(x => x.Ngay).ToList();

            return View("Index", result);
        }
    }
}