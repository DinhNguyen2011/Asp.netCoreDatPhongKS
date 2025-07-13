using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [Authorize]
    public class XuLyTaiKhoanKHController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public XuLyTaiKhoanKHController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem phiếu đặt phòng.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoanId == parsedUserId);

            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return View(new PhieuDatPhong[0]);
            }

            var phieuDatPhongs = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(ct => ct.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .Where(p => p.KhachHangId == khachHang.KhachHangId && p.TrangThai != "Hủy")
                .ToListAsync();

            return View(phieuDatPhongs);
        }

        [HttpGet]
        public async Task<IActionResult> XemPhieuDatPhong(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem chi tiết phiếu.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoanId == parsedUserId);

            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index");
            }

            var phieu = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(ct => ct.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == id && p.KhachHangId == khachHang.KhachHangId);

            if (phieu == null)
            {
                TempData["Error"] = $"Phiếu đặt phòng không tồn tại hoặc không thuộc về bạn. Debug: PhieuDatPhongId={id}, KhachHangId={khachHang.KhachHangId}";
                return RedirectToAction("Index");
            }

            return View(phieu);
        }


            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CancelBooking(int phieuDatPhongId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để hủy phiếu.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoanId == parsedUserId);

            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index");
            }

            var phieu = await _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == phieuDatPhongId && p.KhachHangId == khachHang.KhachHangId);

            if (phieu == null)
            {
                TempData["Error"] = "Phiếu đặt phòng không tồn tại hoặc không thuộc về bạn.";
                return RedirectToAction("Index");
            }

            if (phieu.NgayNhan < DateTime.Now)
            {
                TempData["Error"] = "Không thể hủy phiếu vì đã qua thời gian nhận phòng.";
                return RedirectToAction("Index");
            }

            decimal refundPercentage;
            var hoursDiff = (phieu.NgayNhan - DateTime.Now)?.TotalHours ?? 0;
            if (hoursDiff >= 24)
                refundPercentage = 0.9m; // Hoàn 90%
            else if (hoursDiff >= 12)
                refundPercentage = 0.5m; // Hoàn 50%
            else
                refundPercentage = 0.7m; // Hoàn 70%

            decimal refundAmount = phieu.TongTien * refundPercentage;

            phieu.TrangThai = $"Đã hủy (Hoàn {refundPercentage * 100}% - {String.Format("{0:N0}", refundAmount)} VNĐ)";
            phieu.TinhTrangSuDung = "Đã hủy";

            var phongIds = phieu.ChiTietPhieuPhongs.Select(ct => ct.PhongId).ToList();
            var phongs = await _context.Phongs
                .Where(p => phongIds.Contains(p.PhongId))
                .ToListAsync();

            foreach (var phong in phongs)
            {
                phong.TinhTrang = "Trống";
            }

            _context.Update(phieu);
            _context.UpdateRange(phongs);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Hủy phiếu đặt phòng thành công. Hoàn {refundPercentage * 100}% ({String.Format("{0:N0}", refundAmount)} VNĐ). Vui lòng liên hệ tổng đài 0853461030 để xác nhận hoàn tiền.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> XemDichVu()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem dịch vụ đã sử dụng.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoanId == parsedUserId);

            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return View(new DonHangDichVu[0]);
            }

            var donHangDichVus = await _context.DonHangDichVus
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangDichVus)
                .ThenInclude(ct => ct.DichVu)
                .Where(d => d.KhachHangId == khachHang.KhachHangId)
                .ToListAsync();

            return View(donHangDichVus);
        }

        [HttpGet]
        public async Task<IActionResult> XemHoaDon()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem hóa đơn.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoanId == parsedUserId);

            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return View(new HoaDon[0]);
            }

            var hoaDons = await _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.HoaDonPdps)
                .ThenInclude(hdp => hdp.PhieuDatPhong)
                .ThenInclude(pdp => pdp.ChiTietPhieuPhongs)
                .ThenInclude(ct => ct.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .Include(h => h.HoaDonDichVus)
                .ThenInclude(hddv => hddv.MaDonHangDvNavigation)
                .ThenInclude(dhdv => dhdv.ChiTietDonHangDichVus)
                .ThenInclude(ct => ct.DichVu)
                .Where(h => h.KhachHangId == khachHang.KhachHangId)
                .ToListAsync();

            return View(hoaDons);
        }
    }
}