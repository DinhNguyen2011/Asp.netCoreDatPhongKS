using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Asp.netCoreDatPhongKS.Models.ViewModels;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public HoaDonController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // Action Index: Hiển thị danh sách hóa đơn tổng với bộ lọc và tìm kiếm
        public async Task<IActionResult> Index(string trangThai, string searchString)
        {
            var userName = HttpContext.Session.GetString("Hoten");
            ViewData["Hoten"] = !string.IsNullOrEmpty(userName) ? userName : "Chưa đăng nhập";

            // Lấy danh sách hóa đơn tổng
            var hoaDonsQuery = _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.HoaDonDichVus)
                .ThenInclude(hdv => hdv.MaDonHangDvNavigation)
                .ThenInclude(d => d.ChiTietDonHangDichVus)
                .ThenInclude(c => c.DichVu)
                .Include(h => h.HoaDonPdps)
                .ThenInclude(hdp => hdp.PhieuDatPhong)
                .ThenInclude(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .AsQueryable();

            // Lấy danh sách hóa đơn dịch vụ vãng lai
            var hoaDonDichVusQuery = _context.HoaDonDichVus
                .Include(h => h.MaDonHangDvNavigation)
                .ThenInclude(d => d.KhachHang)
                .Include(h => h.MaDonHangDvNavigation)
                .ThenInclude(d => d.ChiTietDonHangDichVus)
                .ThenInclude(c => c.DichVu)
                .Where(h => h.MaHoaDonTong == null && h.MaDonHangDvNavigation.KhachHangId == null) // Khách vãng lai
                .AsQueryable();

            // Bộ lọc trạng thái
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
            {
                hoaDonsQuery = hoaDonsQuery.Where(h => h.TrangThai == trangThai);
                hoaDonDichVusQuery = hoaDonDichVusQuery.Where(h => h.TrangThaiThanhToan == trangThai);
            }

          
            if (!string.IsNullOrEmpty(searchString))
            {
                hoaDonsQuery = hoaDonsQuery.Where(h => h.KhachHang.HoTen.Contains(searchString) || h.KhachHang.Cccd.Contains(searchString));
                hoaDonDichVusQuery = hoaDonDichVusQuery.Where(h => h.MaDonHangDvNavigation.KhachHang != null &&
                    (h.MaDonHangDvNavigation.KhachHang.HoTen.Contains(searchString) || h.MaDonHangDvNavigation.KhachHang.Cccd.Contains(searchString)));
            }

           
            var hoaDons = await hoaDonsQuery.ToListAsync();
            var hoaDonDichVus = await hoaDonDichVusQuery.ToListAsync();

            var hoaDonViewModels = new List<HoaDonViewModel>();

            // Thêm hóa đơn tổng
            foreach (var hoaDon in hoaDons)
            {
                hoaDonViewModels.Add(new HoaDonViewModel
                {
                    Id = hoaDon.MaHoaDon,
                    Loai = "Hóa đơn tổng",
                    NgayLap = hoaDon.NgayLap,
                    KhachHangTen = hoaDon.KhachHang?.HoTen ?? "Không xác định",
                    KhachHangCCCD = hoaDon.KhachHang?.Cccd ?? "Không có",
                    NhanVienTen = hoaDon.NguoiLapDh ?? "Không xác định",
                    TongTien = hoaDon.TongTien ?? 0,
                    TrangThai = hoaDon.TrangThai,
                    HoaDon = hoaDon,
                    HoaDonDichVu = null
                });
            }

            // Thêm hóa đơn dịch vụ vãng lai
            foreach (var hdv in hoaDonDichVus)
            {
                hoaDonViewModels.Add(new HoaDonViewModel
                {
                    Id = hdv.Id,
                    Loai = "Dịch vụ vãng lai",
                    NgayLap = hdv.NgayThanhToan ?? hdv.MaDonHangDvNavigation.NgayDat,
                    KhachHangTen = "Khách vãng lai",
                    KhachHangCCCD = "Không có",
                    NhanVienTen = "Không xác định", // Khách vãng lai không lưu NhanVienId
                    TongTien = hdv.MaDonHangDvNavigation.ChiTietDonHangDichVus.Sum(c => c.ThanhTien ?? 0),
                    TrangThai = hdv.TrangThaiThanhToan,
                    HoaDon = null,
                    HoaDonDichVu = hdv
                });
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentTrangThai"] = trangThai;

            return View(hoaDonViewModels);
        }

        // Action CreateHoaDonTong: Form tạo hóa đơn tổng khi check-out
        public IActionResult CreateHoaDonTong(string searchKhachHang)
        {
            var userName = HttpContext.Session.GetString("Hoten");
            ViewData["Hoten"] = !string.IsNullOrEmpty(userName) ? userName : "Chưa đăng nhập";

            var khachHangs = _context.KhachHangs.AsQueryable();
            if (!string.IsNullOrEmpty(searchKhachHang))
            {
                khachHangs = khachHangs.Where(k => k.HoTen.Contains(searchKhachHang) || k.Cccd.Contains(searchKhachHang));
            }

            ViewBag.KhachHangs = khachHangs.ToList();

            ViewBag.SearchKhachHang = searchKhachHang;

            return View();
        }

        // Action CreateHoaDonTong POST: Gộp hóa đơn chưa thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHoaDonTong(int khachHangId, int[] hoaDonDichVuIds, int[] phieuDatPhongIds, string hinhThucThanhToan)
        {
            var userName = HttpContext.Session.GetString("Hoten");
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("", "Vui lòng đăng nhập để tạo hóa đơn.");
                return View();
            }

            if (!hoaDonDichVuIds.Any() && !phieuDatPhongIds.Any())
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một hóa đơn dịch vụ hoặc phiếu đặt phòng.");
                ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
                return View();
            }

            if (string.IsNullOrEmpty(hinhThucThanhToan))
            {
                ModelState.AddModelError("", "Vui lòng chọn hình thức thanh toán.");
                ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
                return View();
            }

            // Tạo hóa đơn tổng
            var hoaDon = new HoaDon
            {
                KhachHangId = khachHangId,
                NgayLap = DateTime.Now,
                NguoiLapDh = $"Tên: {userName}",
                TongTienPhong = 0,
                TongTienDichVu = 0,
                TongTien = 0,
                HinhThucThanhToan = hinhThucThanhToan,
                TrangThai = "Đã thanh toán",
                IsKhachVangLai = false,
                GhiChu = null,
                SoTienConNo = 0
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // Gộp hóa đơn dịch vụ
            if (hoaDonDichVuIds.Any())
            {
                var hoaDonDichVus = await _context.HoaDonDichVus
                    .Include(h => h.MaDonHangDvNavigation)
                    .ThenInclude(d => d.ChiTietDonHangDichVus)
                    .Where(h => hoaDonDichVuIds.Contains(h.Id) && h.TrangThaiThanhToan == "Chưa thanh toán")
                    .ToListAsync();

                foreach (var hdv in hoaDonDichVus)
                {
                    hdv.MaHoaDonTong = hoaDon.MaHoaDon;
                    hdv.TrangThaiThanhToan = "Đã thanh toán";
                    hdv.NgayThanhToan = DateTime.Now;
                    hdv.HinhThucThanhToan = hinhThucThanhToan;
                    hdv.MaDonHangDvNavigation.TrangThai = "Đã thanh toán";

                    var tongTienDichVu = hdv.MaDonHangDvNavigation.ChiTietDonHangDichVus.Sum(c => c.ThanhTien ?? 0);
                    hoaDon.TongTienDichVu += tongTienDichVu;
                }
            }

            // Gộp phiếu đặt phòng
            if (phieuDatPhongIds.Any())
            {
                var phieuDatPhongs = await _context.PhieuDatPhongs
                    .Include(p => p.ChiTietPhieuPhongs)
                    .ThenInclude(c => c.Phong)
                    .Where(p => phieuDatPhongIds.Contains(p.PhieuDatPhongId) && p.TinhTrangSuDung == "Đã check-in")
                    .ToListAsync();

                foreach (var phieu in phieuDatPhongs)
                {
                    var chiTietPhieu = phieu.ChiTietPhieuPhongs.FirstOrDefault();
                    if (chiTietPhieu != null)
                    {
                        hoaDon.TongTienPhong += chiTietPhieu.DonGia ?? 0;
                        var hoaDonPdp = new HoaDonPdp
                        {
                            MaHoaDon = hoaDon.MaHoaDon,
                            PhieuDatPhongId = phieu.PhieuDatPhongId
                        };
                        _context.HoaDonPdps.Add(hoaDonPdp);
                        phieu.TinhTrangSuDung = "Đã check-out";
                        phieu.TrangThai = "Đã thanh toán";
                    }
                }
            }

            hoaDon.TongTien = hoaDon.TongTienPhong + hoaDon.TongTienDichVu;
            await _context.SaveChangesAsync();

            return RedirectToAction("PrintHoaDonTong", new { id = hoaDon.MaHoaDon });
        }

        // Action PrintHoaDonTong: In hóa đơn tổng
        public async Task<IActionResult> PrintHoaDonTong(int id)
        {
            var taiKhoanId = HttpContext.Session.GetInt32("TaiKhoanId");
            var userName = HttpContext.Session.GetString("Hoten");
            
            ViewData["Hoten"] = !string.IsNullOrEmpty(userName) ? userName : "Chưa đăng nhập";
            ViewData["HotelName"] = "Khách sạn THIỀM ĐỊNH";
            ViewData["HotelAddress"] = "180 Cao Lỗ, Quận 8, TP. Hồ Chí Minh";

            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.HoaDonDichVus)
                .ThenInclude(hdv => hdv.MaDonHangDvNavigation)
                .ThenInclude(d => d.ChiTietDonHangDichVus)
                .ThenInclude(c => c.DichVu)
                .Include(h => h.HoaDonPdps)
                .ThenInclude(hdp => hdp.PhieuDatPhong)
                .ThenInclude(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null)
                return NotFound();

            return View(hoaDon);
        }
        public async Task<IActionResult> Edit(int id)
        {
          
            var userName = HttpContext.Session.GetString("Hoten");
            ViewData["Hoten"] = !string.IsNullOrEmpty(userName) ? userName : "Chưa đăng nhập";

            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachHang)
               
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            if (hoaDon.TrangThai == "Hủy")
            {
                ModelState.AddModelError("", "Không thể sửa hóa đơn đã hủy.");
                return View(hoaDon);
            }

            ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
        
            ViewBag.HinhThucThanhToans = new List<string> { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" }; // Tùy chỉnh theo nghiệp vụ

            return View(hoaDon);
        }

        // Action Edit: Lưu thay đổi hóa đơn tổng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HoaDon hoaDon)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }

            if (id != hoaDon.MaHoaDon)
            {
                return BadRequest();
            }

            var existingHoaDon = await _context.HoaDons
                .Include(h => h.HoaDonDichVus)
                .Include(h => h.HoaDonPdps)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (existingHoaDon == null)
            {
                return NotFound();
            }

            if (existingHoaDon.TrangThai == "Hủy")
            {
                ModelState.AddModelError("", "Không thể sửa hóa đơn đã hủy.");
                ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
             
                ViewBag.HinhThucThanhToans = new List<string> { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" };
                return View(hoaDon);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Chỉ cập nhật các trường được phép sửa
                    existingHoaDon.KhachHangId = hoaDon.KhachHangId;
                  //  existingHoaDon.NguoiLapDh = hoaDon.NguoiLapDh;
                    existingHoaDon.HinhThucThanhToan = hoaDon.HinhThucThanhToan;
                    existingHoaDon.TrangThai = hoaDon.TrangThai;
                    existingHoaDon.GhiChu = hoaDon.GhiChu;
                    existingHoaDon.SoTienConNo = hoaDon.SoTienConNo;

                    // Không sửa TongTien, TongTienPhong, TongTienDichVu (tính lại nếu cần)
                    // existingHoaDon.TongTien = existingHoaDon.HoaDonDichVus.Sum(hdv => 
                    //     hdv.MaDonHangDvNavigation.ChiTietDonHangDichVus.Sum(c => c.ThanhTien ?? 0)) +
                    //     existingHoaDon.HoaDonPdps.Sum(hdp => 
                    //     hdp.PhieuDatPhong.ChiTietPhieuPhongs.FirstOrDefault()?.DonGia ?? 0);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHoaDon))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            ViewBag.HinhThucThanhToans = new List<string> { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" };
            return View(hoaDon);
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHoaDon == id);
        }

        // Action GetHoaDonDichVu: Lấy hóa đơn dịch vụ chưa thanh toán theo khách hàng
        [HttpGet]
        public async Task<IActionResult> GetHoaDonDichVu(int khachHangId)
        {
            var hoaDonDichVus = await _context.HoaDonDichVus
                .Include(h => h.MaDonHangDvNavigation)
                .ThenInclude(d => d.ChiTietDonHangDichVus)
                .Where(h => h.MaDonHangDvNavigation.KhachHangId == khachHangId && h.TrangThaiThanhToan == "Chưa thanh toán")
                .Select(h => new
                {
                    id = h.Id,
                    maDonHangDv = h.MaDonHangDv,
                    tongTien = h.MaDonHangDvNavigation.ChiTietDonHangDichVus.Sum(c => c.ThanhTien ?? 0)
                })
                .ToListAsync();

            return Json(hoaDonDichVus);
        }

        // Action GetPhieuDatPhong: Lấy phiếu đặt phòng chưa thanh toán theo khách hàng
        [HttpGet]
        public async Task<IActionResult> GetPhieuDatPhong(int khachHangId)
        {
            var phieuDatPhongs = await _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .Where(p => p.KhachHangId == khachHangId && p.TrangThai == "Chưa thanh toán" && p.TinhTrangSuDung == "Đã check-in")
                .Select(p => new
                {
                    phieuDatPhongId = p.PhieuDatPhongId,
                    maPhieu = p.MaPhieu,
                    soPhong = p.ChiTietPhieuPhongs.FirstOrDefault().Phong.SoPhong,
                    donGia = p.ChiTietPhieuPhongs.FirstOrDefault().DonGia ?? 0
                })
                .ToListAsync();

            return Json(phieuDatPhongs);
        }
    }
}