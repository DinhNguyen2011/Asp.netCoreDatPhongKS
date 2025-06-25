using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.netCoreDatPhongKS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Asp.netCoreDatPhongKS.Controllers
{
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
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem phiếu đặt phòng.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoan.Email == email);

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
                .Where(p => p.KhachHangId == khachHang.KhachHangId && p.TrangThai != "Hủy" )
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
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem chi tiết phiếu.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoan.Email == email);

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
                TempData["Error"] = "Phiếu đặt phòng không tồn tại hoặc không thuộc về bạn.";
                return RedirectToAction("Index");
            }

            return View(phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int phieuDatPhongId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để hủy phiếu.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoan.Email == email);

            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index");
            }

            var phieu = await _context.PhieuDatPhongs
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

            phieu.TrangThai = $"Đã hủy (Hoàn {refundPercentage * 100}%)";
            phieu.TinhTrangSuDung = "Đã check-out";
            _context.Update(phieu);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Hủy phiếu đặt phòng thành công. Hoàn {refundPercentage * 100}% - Vui lòng liên hệ tổng đài 0853461030 để xác nhận hoàn tiền.";
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
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Vui lòng đăng nhập để xem dịch vụ đã sử dụng.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.TaiKhoan.Email == email);

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
    }
}