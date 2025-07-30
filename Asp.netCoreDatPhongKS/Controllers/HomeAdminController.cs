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
                .Where(ct => ct.PhieuDatPhong != null && ct.PhieuDatPhong.TinhTrangSuDung == "Đã check-in")
                .Select(ct => ct.PhongId).Distinct().Count(); // Phòng đang sử dụng (Đã check-in)
            ViewBag.AvailableRooms = _context.Phongs
                .Where(p => p.TinhTrang == "Trống")
                .Count(); // Phòng trống

            //ViewBag.BookedRooms = _context.PhieuDatPhongs
            //    .Where(p => p.TinhTrangSuDung == "Đã đặt" && p.NgayNhan <= DateTime.Now && p.NgayTra >= DateTime.Now)
            //    .SelectMany(p => p.ChiTietPhieuPhongs)
            //    .Select(ct => ct.PhongId).Distinct().Count(); // Phòng đã đặt nhưng chưa check-in
            ViewBag.BookedRooms = _context.Phongs
              .Where(p => p.TinhTrang == "Đã đặt")
              .Count(); // Phòng trống

            // Thống kê khách hàng
            ViewBag.TotalCustomers = _context.KhachHangs.Count(); // Tổng số khách

            // Thống kê tổng đơn đặt (Đã check-out)
            ViewBag.TotalBookings = _context.PhieuDatPhongs
                .Count(p => p.TinhTrangSuDung == "Đã check-out"); // Tổng đơn đã check-out

            // Thống kê doanh thu tháng này
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.MonthlyRevenue = _context.HoaDons
                .Where(h => h.NgayLap.HasValue && h.NgayLap.Value >= startOfMonth && h.NgayLap.Value <= DateTime.Now)
                .Sum(h => h.TongTien); // Doanh thu tháng này, xử lý null

            // Thống kê nhân viên
            ViewBag.TotalStaff = _context.NhanViens.Count(); // Tổng số nhân viên

            // Dữ liệu cho biểu đồ phân loại phòng
            var roomTypes = _context.Phongs
                .GroupBy(p => p.LoaiPhong.TenLoai)
                .Select(g => new { Type = g.Key ?? "Không xác định", Count = g.Count() })
                .ToList();
            ViewBag.RoomTypeLabels = roomTypes.Select(r => r.Type).ToList();
            ViewBag.RoomTypeData = roomTypes.Select(r => r.Count).ToList();

            // Tính tổng doanh thu từ HoaDon (bao gồm cả PDP và dịch vụ)
            var totalRevenueData = _context.HoaDons
                .Where(hd => hd.NgayLap.HasValue)
                .GroupBy(hd => 1) // Nhóm tất cả vào một nhóm để tính tổng
                .Select(g => new
                {
                    TongTienPhong = g.Sum(hd => hd.TongTienPhong ?? 0),
                    TongTienDichVu = g.Sum(hd => hd.TongTienDichVu ?? 0)
                })
                .FirstOrDefault();

            ViewBag.RevenueLabels = new[] { "Doanh thu từ phòng", "Doanh thu từ dịch vụ" };
            ViewBag.RevenueData = new[] { totalRevenueData?.TongTienPhong ?? 0, totalRevenueData?.TongTienDichVu ?? 0 };

            return View();
        }
    }
}