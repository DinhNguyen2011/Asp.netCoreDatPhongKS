using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Thêm để sử dụng LINQ

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class PhieuDatPhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public PhieuDatPhongController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: Hiển thị danh sách phiếu đặt phòng
        public async Task<IActionResult> Index()
        {
            try
            {
                var phieuDatPhongs = await _context.PhieuDatPhongs
                    .Include(p => p.KhachHang)
                    .Include(p => p.ChiTietPhieuPhongs)
                    .ThenInclude(c => c.Phong)
                    .ToListAsync();
                return View(phieuDatPhongs);
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách phiếu.";
                return View(new List<PhieuDatPhong>());
            }
        }

        [AuthorizePermission("ManagePhieuDatPhong")]
        public IActionResult Create()
        {
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.Phongs = GetAvailableRooms(null, null);
            return View();
        }

        private List<Phong> GetAvailableRooms(DateTime? newNgayNhan, DateTime? newNgayTra)
        {
            var allRooms = _context.Phongs.ToList();
            var availableRooms = new List<Phong>();

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .Where(p => p.NgayNhan != null && p.NgayTra != null)
                .SelectMany(p => p.ChiTietPhieuPhongs.Select(c => new { c.PhongId, p.NgayNhan, p.NgayTra }))
                .ToList();

            foreach (var room in allRooms)
            {
                bool isAvailable = true;
                foreach (var booking in bookedRooms)
                {
                    if (booking.PhongId == room.PhongId)
                    {
                        DateTime bookingStart = (DateTime)booking.NgayNhan;
                        DateTime bookingEnd = (DateTime)booking.NgayTra;

                        if (newNgayNhan.HasValue && newNgayTra.HasValue)
                        {
                            DateTime newStart = newNgayNhan.Value;
                            DateTime newEnd = newNgayTra.Value;

                            if (!(newEnd < bookingStart || newStart > bookingEnd))
                            {
                                isAvailable = false;
                                break;
                            }
                        }
                    }
                }
                if (isAvailable)
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> Create(PhieuDatPhong model, List<int> phongIds)
        {
            if (!ModelState.IsValid || phongIds == null || !phongIds.Any())
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin và chọn ít nhất một phòng.";
                ViewBag.KhachHangs = _context.KhachHangs.ToList();
                ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra);
                return View(model);
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    model.MaPhieu = $"PDP-{DateTime.Now:yyyyMMddHHmmss}";
                    model.NgayDat = DateTime.Now;
                    model.TrangThai = "Chưa thanh toán";
                    model.TinhTrangSuDung = "Chưa sử dụng";
                    model.SoTienCoc = 0;
                    model.SoTienDaThanhToan = 0;

                    decimal tongTienPhong = 0;
                    var chiTietPhieuPhongs = new List<ChiTietPhieuPhong>();
                    foreach (var phongId in phongIds)
                    {
                        var phong = await _context.Phongs.FindAsync(phongId);
                        if (phong == null)
                        {
                            TempData["Error"] = $"Phòng {phongId} không tồn tại.";
                            ViewBag.KhachHangs = _context.KhachHangs.ToList();
                            ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra);
                            return View(model);
                        }

                        // Kiểm tra phòng có khả dụng không
                        bool isAvailable = IsRoomAvailable(phongId, model.NgayNhan, model.NgayTra);
                        if (!isAvailable)
                        {
                            // Thêm log để kiểm tra
                            var conflictingBookings = _context.PhieuDatPhongs
                                .Include(p => p.ChiTietPhieuPhongs)
                                .Where(p => p.NgayNhan != null && p.NgayTra != null)
                                .SelectMany(p => p.ChiTietPhieuPhongs.Where(c => c.PhongId == phongId)
                                    .Select(c => new { p.NgayNhan, p.NgayTra }))
                                .ToList();

                            string conflictDetails = "Các khoảng thời gian xung đột: ";
                            foreach (var booking in conflictingBookings)
                            {
                                conflictDetails += $"[Từ {booking.NgayNhan:yyyy-MM-dd} đến {booking.NgayTra:yyyy-MM-dd}] ";
                            }

                            TempData["Error"] = $"Phòng {phongId} không khả dụng vào thời gian này. {conflictDetails}";
                            ViewBag.KhachHangs = _context.KhachHangs.ToList();
                            ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra);
                            return View(model);
                        }

                        phong.TinhTrang = "Chưa thanh toán";
                        _context.Phongs.Update(phong);

                        var soNgay = (model.NgayTra - model.NgayNhan)?.Days ?? 1;
                        var donGia = phong.GiaPhong1Dem ?? 0;
                        if (donGia == 0)
                        {
                            TempData["Error"] = $"Phòng {phongId} không có giá hoặc giá không hợp lệ.";
                            ViewBag.KhachHangs = _context.KhachHangs.ToList();
                            ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra);
                            return View(model);
                        }

                        var giaPhong = donGia * soNgay;
                        tongTienPhong += giaPhong;

                        chiTietPhieuPhongs.Add(new ChiTietPhieuPhong
                        {
                            PhieuDatPhongId = model.PhieuDatPhongId,
                            PhongId = phongId,
                            DonGia = donGia
                        });
                    }

                    model.ChiTietPhieuPhongs = chiTietPhieuPhongs;
                    model.TongTien = tongTienPhong;

                    _context.PhieuDatPhongs.Add(model);
                    await _context.SaveChangesAsync();

                    foreach (var chiTiet in model.ChiTietPhieuPhongs)
                    {
                        chiTiet.PhieuDatPhongId = model.PhieuDatPhongId;
                        _context.ChiTietPhieuPhongs.Update(chiTiet);
                    }
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["Success"] = "Tạo phiếu đặt phòng thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra.";
                ViewBag.KhachHangs = _context.KhachHangs.ToList();
                ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra);
                return View(model);
            }
        }

        private bool IsRoomAvailable(int phongId, DateTime? newNgayNhan, DateTime? newNgayTra)
        {
            if (!newNgayNhan.HasValue || !newNgayTra.HasValue)
                return false;

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .Where(p => p.NgayNhan != null && p.NgayTra != null)
                .SelectMany(p => p.ChiTietPhieuPhongs.Where(c => c.PhongId == phongId)
                    .Select(c => new { p.NgayNhan, p.NgayTra }))
                .ToList();

            DateTime newStart = newNgayNhan.Value;
            DateTime newEnd = newNgayTra.Value;

            foreach (var booking in bookedRooms)
            {
                DateTime bookingStart = (DateTime)booking.NgayNhan;
                DateTime bookingEnd = (DateTime)booking.NgayTra;

                if (!(newEnd < bookingStart || newStart > bookingEnd))
                {
                    return false;
                }
            }
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public IActionResult FilterEmptyRooms(DateTime? ngayNhan, DateTime? ngayTra)
        {
            if (!ngayNhan.HasValue || !ngayTra.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng nhập ngày nhận và ngày trả." });
            }

            var availableRooms = GetAvailableRooms(ngayNhan, ngayTra)
                .Select(p => new { p.PhongId, p.SoPhong, p.GiaPhong1Dem, p.TinhTrang })
                .ToList();

            return Json(new { success = true, rooms = availableRooms });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> AddKhachHang([FromBody] KhachHang khachHang)
        {
            try
            {
                if (string.IsNullOrEmpty(khachHang.HoTen) || string.IsNullOrEmpty(khachHang.Email) ||
                    string.IsNullOrEmpty(khachHang.SoDienThoai) || string.IsNullOrEmpty(khachHang.DiaChi) ||
                    string.IsNullOrEmpty(khachHang.Cccd))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ họ tên, email, số điện thoại, địa chỉ và CCCD." });
                }

                var existingTaiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == khachHang.Email);
                if (existingTaiKhoan != null)
                {
                    return Json(new { success = false, message = "Email đã được sử dụng cho tài khoản khác." });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var taiKhoan = new TaiKhoan
                    {
                        Email = khachHang.Email,
                        Hoten = khachHang.HoTen,
                        MatKhau = BCrypt.Net.BCrypt.HashPassword("1"),
                        VaiTroId = 3,
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };

                    _context.TaiKhoans.Add(taiKhoan);
                    await _context.SaveChangesAsync();

                    var newKhachHang = new KhachHang
                    {
                        HoTen = khachHang.HoTen,
                        Email = khachHang.Email,
                        SoDienThoai = khachHang.SoDienThoai,
                        DiaChi = khachHang.DiaChi,
                        Cccd = khachHang.Cccd,
                        GhiChu = khachHang.GhiChu,
                        TaiKhoanId = taiKhoan.TaiKhoanId,
                        NgayTao = DateTime.Now
                    };

                    _context.KhachHangs.Add(newKhachHang);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Json(new
                    {
                        success = true,
                        khachHang = new
                        {
                            khachHangId = newKhachHang.KhachHangId,
                            hoTen = newKhachHang.HoTen,
                            email = newKhachHang.Email
                        },
                        clearForm = true
                    });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }
    }
}