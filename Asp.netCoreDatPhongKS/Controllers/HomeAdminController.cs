using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public HomeAdminController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            string email = HttpContext.Session.GetString("Email");
            if (!string.IsNullOrEmpty(email))
            {
                ViewData["Email"] = email;
            }

            // Thống kê số phòng
            ViewBag.TotalRooms = _context.Phongs.Count(); // Tổng số phòng
            ViewBag.OccupiedRooms = _context.ChiTietPhieuPhongs
                .Where(ct => ct.PhieuDatPhong != null && ct.PhieuDatPhong.TinhTrangSuDung != "Đã hủy")
                .Select(ct => ct.PhongId).Distinct().Count(); // Phòng đang sử dụng
            ViewBag.BookedRooms = _context.PhieuDatPhongs
                .Where(p => p.TinhTrangSuDung == "Đã check-out" && p.NgayNhan <= DateTime.Now && p.NgayTra >= DateTime.Now)
                .SelectMany(p => p.ChiTietPhieuPhongs)
                .Select(ct => ct.PhongId).Distinct().Count(); // Phòng đã đặt
            ViewBag.AvailableRooms = ViewBag.TotalRooms - ViewBag.OccupiedRooms; // Phòng trống

            // Thống kê khách hàng
            ViewBag.TotalCustomers = _context.KhachHangs.Count(); // Tổng số khách

            // Thống kê đơn đặt phòng
            ViewBag.TotalBookings = _context.PhieuDatPhongs.Count(p => p.TinhTrangSuDung != "Đã hủy"); // Tổng đơn đặt

            // Thống kê doanh thu tháng này
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.MonthlyRevenue = _context.HoaDons
                .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= startOfMonth && h.NgayLap.Value <= DateTime.Now)
                .Sum(h => h.TongTien); // Doanh thu tháng này

            // Thống kê nhân viên
            ViewBag.TotalStaff = _context.NhanViens.Count(); // Tổng số nhân viên

            // Dữ liệu cho biểu đồ phân loại phòng
            var roomTypes = _context.Phongs
                .GroupBy(p => p.LoaiPhong.TenLoai)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();
            ViewBag.RoomTypeLabels = roomTypes.Select(r => r.Type).ToList();
            ViewBag.RoomTypeData = roomTypes.Select(r => r.Count).ToList();

            // Dữ liệu cho biểu đồ tỉ lệ đặt phòng theo loại
            var bookingRatios = _context.PhieuDatPhongs
                .Where(p => p.TinhTrangSuDung != "Đã hủy" && p.NgayNhan <= DateTime.Now && p.NgayTra >= DateTime.Now)
                .Join(_context.ChiTietPhieuPhongs,
                      p => p.PhieuDatPhongId,
                      ct => ct.PhieuDatPhongId,
                      (p, ct) => new { ct.Phong.LoaiPhong.TenLoai })
                .GroupBy(x => x.TenLoai)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();
            ViewBag.BookingRatioLabels = bookingRatios.Select(r => r.Type).ToList();
            ViewBag.BookingRatioData = bookingRatios.Select(r => r.Count).ToList();

            return View();
        }
    }
}