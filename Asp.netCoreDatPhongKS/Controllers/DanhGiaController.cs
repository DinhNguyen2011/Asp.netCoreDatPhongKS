using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [Authorize]

    public class DanhGiaController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public DanhGiaController(HotelPlaceVipContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var danhGias = await _context.DanhGia
                .Include(d => d.Phong)
                .Include(d => d.DichVu)
                .Include(d => d.PhieuDatPhong)
                .Include(d => d.DonHangDichVu)
                .Include(d => d.TaiKhoan)
                .ToListAsync();
            return View(danhGias);
        }

        public IActionResult Phong(int phieuDatPhongId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["Error"] = "Vui lòng đăng nhập để đánh giá.";
                return RedirectToAction("Index", "XuLyTaiKhoanKH");
            }

            // Ánh xạ TaiKhoanId sang KhachHangId
            var khachHang = _context.KhachHangs
                .Where(kh => kh.TaiKhoanId == parsedUserId)
                .Select(kh => kh.KhachHangId)
                .FirstOrDefault();

            if (khachHang == 0)
            {
                TempData["Error"] = "Không tìm thấy khách hàng tương ứng với tài khoản.";
                return RedirectToAction("Index", "XuLyTaiKhoanKH");
            }

            var phieu = _context.PhieuDatPhongs
                .Where(p => p.PhieuDatPhongId == phieuDatPhongId && p.KhachHangId == khachHang && p.TinhTrangSuDung.Trim() == "Đã check-out")
                .Select(p => new DanhGiaViewModel
                {
                    PhieuDatPhongId = p.PhieuDatPhongId,
                    MaPhieu = p.MaPhieu,
                    NgayNhan = p.NgayNhan,
                    NgayTra = p.NgayTra,
                    Phongs = (from ctp in _context.ChiTietPhieuPhongs
                              join ph in _context.Phongs on ctp.PhongId equals ph.PhongId
                              where ctp.PhieuDatPhongId == p.PhieuDatPhongId
                              let danhGia = _context.DanhGia.FirstOrDefault(d => d.PhieuDatPhongId == p.PhieuDatPhongId && d.PhongId == ph.PhongId)
                              select new PhongDanhGia
                              {
                                  PhongId = ph.PhongId,
                                  SoPhong = ph.SoPhong,
                                  DanhGiaId = danhGia != null ? danhGia.DanhGiaId : 0,
                                  Diem = danhGia != null ? danhGia.Diem : null,
                                  NoiDung = danhGia != null ? danhGia.NoiDung : null,
                                  PhieuDatPhongId = p.PhieuDatPhongId
                              }).ToList()
                })
                .FirstOrDefault();

            if (phieu == null)
            {
                var phieuDebug = _context.PhieuDatPhongs
                    .Where(p => p.PhieuDatPhongId == phieuDatPhongId)
                    .Select(p => new { p.PhieuDatPhongId, p.KhachHangId, p.TinhTrangSuDung })
                    .FirstOrDefault();
                TempData["Error"] = $"Phiếu đặt phòng không hợp lệ hoặc chưa được sử dụng. Debug: PhieuDatPhongId={phieuDatPhongId}, KhachHangId={khachHang}, TinhTrangSuDung={(phieuDebug != null ? $"\"{phieuDebug.TinhTrangSuDung}\"" : "Không tìm thấy")}";
                return RedirectToAction("Index", "XuLyTaiKhoanKH");
            }

            return View(phieu);
        }

        public IActionResult DichVu(int donHangDichVuId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                TempData["Error"] = "Vui lòng đăng nhập để đánh giá.";
                return RedirectToAction("XemDichVu", "XuLyTaiKhoanKH");
            }

            // Ánh xạ TaiKhoanId sang KhachHangId
            var khachHang = _context.KhachHangs
                .Where(kh => kh.TaiKhoanId == parsedUserId)
                .Select(kh => kh.KhachHangId)
                .FirstOrDefault();

            if (khachHang == 0)
            {
                TempData["Error"] = "Không tìm thấy khách hàng tương ứng với tài khoản.";
                return RedirectToAction("XemDichVu", "XuLyTaiKhoanKH");
            }

            var donHang = _context.DonHangDichVus
                .Where(d => d.MaDonHangDv == donHangDichVuId && d.KhachHangId == khachHang && d.TrangThai.Trim() == "Đã thanh toán")
                .Select(d => new DanhGiaDichVuViewModel
                {
                    MaDonHangDv = d.MaDonHangDv,
                    NgayDat = d.NgayDat,
                    DichVus = (from ctdv in _context.ChiTietDonHangDichVus
                               join dv in _context.DichVus on ctdv.DichVuId equals dv.DichVuId
                               where ctdv.MaDonHangDv == d.MaDonHangDv
                               let danhGia = _context.DanhGia.FirstOrDefault(dg => dg.DonHangDichVuId == d.MaDonHangDv && dg.DichVuId == dv.DichVuId)
                               select new DichVuDanhGia
                               {
                                   DichVuId = dv.DichVuId,
                                   TenDichVu = dv.TenDichVu,
                                   SoLuong = ctdv.SoLuong,
                                   DanhGiaId = danhGia != null ? danhGia.DanhGiaId : 0,
                                   Diem = danhGia != null ? danhGia.Diem : null,
                                   NoiDung = danhGia != null ? danhGia.NoiDung : null,
                                   MaDonHangDv = d.MaDonHangDv
                               }).ToList()
                })
                .FirstOrDefault();

            if (donHang == null)
            {
                var donHangDebug = _context.DonHangDichVus
                    .Where(d => d.MaDonHangDv == donHangDichVuId)
                    .Select(d => new { d.MaDonHangDv, d.KhachHangId, d.TrangThai })
                    .FirstOrDefault();
                TempData["Error"] = $"Đơn hàng dịch vụ không hợp lệ hoặc chưa được thanh toán. Debug: MaDonHangDv={donHangDichVuId}, TaiKhoanId={parsedUserId}, KhachHangId={khachHang}, TrangThai={(donHangDebug != null ? $"\"{donHangDebug.TrangThai}\"" : "Không tìm thấy")}";
                return RedirectToAction("XemDichVu", "XuLyTaiKhoanKH");
            }

            return View(donHang);
        }

        [HttpPost]
        public IActionResult SubmitPhong([FromBody] PhongDanhGia model)
        {
            if (model.Diem < 1 || model.Diem > 5)
            {
                return Json(new { success = false, message = "Điểm phải từ 1 đến 5." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để đánh giá." });
            }

            if (!_context.ChiTietPhieuPhongs.Any(ctp => ctp.PhieuDatPhongId == model.PhieuDatPhongId && ctp.PhongId == model.PhongId))
            {
                return Json(new { success = false, message = "Phòng không thuộc phiếu đặt phòng này." });
            }

            if (_context.DanhGia.Any(d => d.PhieuDatPhongId == model.PhieuDatPhongId && d.PhongId == model.PhongId))
            {
                return Json(new { success = false, message = "Phòng này đã được đánh giá cho phiếu đặt phòng này." });
            }

            var danhGia = new DanhGium
            {
                PhongId = model.PhongId,
                PhieuDatPhongId = model.PhieuDatPhongId,
                Diem = model.Diem,
                NoiDung = model.NoiDung,
                NgayDanhGia = DateTime.Now,
                TaiKhoanId = parsedUserId
            };

            _context.DanhGia.Add(danhGia);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult SubmitDichVu([FromBody] DichVuDanhGia model)
        {
            if (model.Diem < 1 || model.Diem > 5)
            {
                return Json(new { success = false, message = "Điểm phải từ 1 đến 5." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để đánh giá." });
            }

            if (!_context.ChiTietDonHangDichVus.Any(ct => ct.MaDonHangDv == model.MaDonHangDv && ct.DichVuId == model.DichVuId))
            {
                return Json(new { success = false, message = "Dịch vụ không thuộc đơn hàng này." });
            }

            if (_context.DanhGia.Any(d => d.DonHangDichVuId == model.MaDonHangDv && d.DichVuId == model.DichVuId))
            {
                return Json(new { success = false, message = "Dịch vụ này đã được đánh giá cho đơn hàng này." });
            }

            var danhGia = new DanhGium
            {
                DichVuId = model.DichVuId,
                DonHangDichVuId = model.MaDonHangDv,
                Diem = model.Diem,
                NoiDung = model.NoiDung,
                NgayDanhGia = DateTime.Now,
                TaiKhoanId = parsedUserId
            };

            _context.DanhGia.Add(danhGia);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}