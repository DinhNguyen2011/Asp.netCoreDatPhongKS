using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    .ThenInclude(p => p.LoaiPhong)
                    .ToListAsync();
                return View(phieuDatPhongs);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải danh sách phiếu: {ex.Message}";
                return View(new List<PhieuDatPhong>());
            }
        }

        [AuthorizePermission("ManagePhieuDatPhong")]
        public IActionResult Create()
        {
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.Phongs = GetAvailableRooms(null, null, null);
            return View();
        }

        private List<Phong> GetAvailableRooms(DateTime? newNgayNhan, DateTime? newNgayTra, int? soLuongKhach)
        {
            var query = _context.Phongs
                .Include(p => p.LoaiPhong)
                .AsQueryable();

            if (soLuongKhach.HasValue)
            {
                query = query.Where(p => p.SoLuongKhach >= soLuongKhach);
            }

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .Where(p => p.NgayNhan != null && p.NgayTra != null && p.TrangThai != "Hủy" && p.TrangThai != "Hoàn thành")
                .SelectMany(p => p.ChiTietPhieuPhongs.Select(c => new { c.PhongId, p.NgayNhan, p.NgayTra }))
                .ToList();

            var availableRooms = query.ToList().Where(room =>
            {
                if (!newNgayNhan.HasValue || !newNgayTra.HasValue) return true;
                return !bookedRooms.Any(booking =>
                    booking.PhongId == room.PhongId &&
                    !(newNgayTra.Value <= booking.NgayNhan || newNgayNhan.Value >= booking.NgayTra));
            }).ToList();

            return availableRooms;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> Create(PhieuDatPhong model, List<int> phongIds, decimal soTienCoc, decimal? soTienDaThanhToan)
        {
            if (!ModelState.IsValid || phongIds == null || !phongIds.Any())
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin và chọn ít nhất một phòng.";
                ViewBag.KhachHangs = _context.KhachHangs.ToList();
                ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra, null);
                return View(model);
            }
            IDbContextTransaction transaction = null;
            try
            {
                transaction = await _context.Database.BeginTransactionAsync();
                foreach (var phongId in phongIds)
                {
                    var phong = await _context.Phongs.FindAsync(phongId);
                    if (phong == null)
                    {
                        TempData["Error"] = $"Phòng {phongId} không tồn tại.";
                        throw new Exception($"Phòng {phongId} không tồn tại.");
                    }

                    if (!IsRoomAvailable(phongId, model.NgayNhan, model.NgayTra))
                    {
                        TempData["Error"] = $"Phòng {phong.SoPhong} không khả dụng.";
                        throw new Exception($"Phòng {phong.SoPhong} không khả dụng.");
                    }

                    var donGia = phong.GiaPhong1Dem ?? 0;
                    if (donGia <= 0)
                    {
                        TempData["Error"] = $"Phòng {phong.SoPhong} có giá không hợp lệ.";
                        throw new Exception($"Phòng {phong.SoPhong} có giá không hợp lệ.");
                    }

                    var soNgay = (model.NgayTra - model.NgayNhan)?.Days ?? 1;
                    var tongTien = donGia * soNgay;

                    var phieu = new PhieuDatPhong
                    {
                        MaPhieu = $"PDP-{DateTime.Now:yyyyMMddHHmmss}-{phongId}",
                        KhachHangId = model.KhachHangId,
                        NgayDat = DateTime.Now,
                        NgayNhan = model.NgayNhan,
                        NgayTra = model.NgayTra,
                        TongTien = tongTien,
                        SoTienCoc = soTienCoc / phongIds.Count, // Chia đều tiền cọc
                        SoTienDaThanhToan = soTienDaThanhToan.HasValue ? soTienDaThanhToan.Value / phongIds.Count : 0,
                        TrangThai = soTienDaThanhToan.HasValue && soTienDaThanhToan.Value >= tongTien ? "Đã thanh toán" : "Chưa thanh toán",
                        TinhTrangSuDung = "Chưa sử dụng",
                        ChiTietPhieuPhongs = new List<ChiTietPhieuPhong>
                        {
                            new ChiTietPhieuPhong
                            {
                                PhongId = phongId,
                                DonGia = donGia
                            }
                        }
                    };

                    _context.PhieuDatPhongs.Add(phieu);
                    phong.TinhTrang = "Đã đặt";
                    _context.Phongs.Update(phong);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Tạo phiếu đặt phòng thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                ViewBag.KhachHangs = _context.KhachHangs.ToList();
                ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra, null);
                return View(model);
            }
        }

        private bool IsRoomAvailable(int phongId, DateTime? newNgayNhan, DateTime? newNgayTra)
        {
            if (!newNgayNhan.HasValue || !newNgayTra.HasValue) return false;

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .Where(p => p.NgayNhan != null && p.NgayTra != null && p.TrangThai != "Hủy" && p.TrangThai != "Hoàn thành")
                .SelectMany(p => p.ChiTietPhieuPhongs.Where(c => c.PhongId == phongId)
                    .Select(c => new { p.NgayNhan, p.NgayTra }))
                .ToList();

            return !bookedRooms.Any(booking =>
                !(newNgayTra.Value <= booking.NgayNhan || newNgayNhan.Value >= booking.NgayTra));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public IActionResult FilterEmptyRooms(DateTime? ngayNhan, DateTime? ngayTra, int? soLuongKhach)
        {
            if (!ngayNhan.HasValue || !ngayTra.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng nhập ngày nhận và ngày trả." });
            }

            var availableRooms = GetAvailableRooms(ngayNhan, ngayTra, soLuongKhach)
                .Select(p => new
                {
                    p.PhongId,
                    p.SoPhong,
                    p.GiaPhong1Dem,
                    p.TinhTrang,
                    p.HinhAnh,
                    p.SoLuongKhach,
                    LoaiPhong = p.LoaiPhong?.TenLoai,
                    HinhAnh1 = p.HinhAnh1,
                    HinhAnh2 = p.HinhAnh2,
                    HinhAnh3 = p.HinhAnh3
                })
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
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
                }

                var existingTaiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == khachHang.Email);
                if (existingTaiKhoan != null)
                {
                    return Json(new { success = false, message = "Email đã được sử dụng." });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPhieuDatPhongDetails(int phieuDatPhongId)
        {
            var phieu = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == phieuDatPhongId);

            if (phieu == null)
            {
                return Json(new { success = false, message = "Phiếu đặt phòng không tồn tại." });
            }

            var chiTiet = phieu.ChiTietPhieuPhongs.FirstOrDefault();
            if (chiTiet == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin phòng." });
            }

            var soNgay = (phieu.NgayTra - phieu.NgayNhan)?.Days ?? 1;

            return Json(new
            {
                success = true,
                phieu = new
                {
                    maPhieu = phieu.MaPhieu,
                    khachHang = new
                    {
                        hoTen = phieu.KhachHang?.HoTen ?? "Không xác định",
                        email = phieu.KhachHang?.SoDienThoai ?? "Không có"
                    },
                    chiTiet = new
                    {
                        soPhong = chiTiet.Phong?.SoPhong ?? "Không xác định",
                        loaiPhong = chiTiet.Phong?.LoaiPhong?.TenLoai ?? "Không xác định",
                        donGia = chiTiet.DonGia ?? 0
                    },
                    ngayNhan = phieu.NgayNhan,
                    ngayTra = phieu.NgayTra,
                    soNgay = soNgay,
                    tongTien = phieu.TongTien ?? 0,
                    soTienCoc = phieu.SoTienCoc ?? 0,
                    soTienDaThanhToan = phieu.SoTienDaThanhToan ?? 0,
                    trangThai = phieu.TrangThai,
                    tinhTrangSuDung = phieu.TinhTrangSuDung
                }
            });
        }

        // Existing action for room details (used in Create.cshtml)
        [HttpGet]
        public async Task<IActionResult> GetRoomDetails(int phongId)
        {
            var room = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.PhongId == phongId);

            if (room == null)
            {
                return Json(new { success = false, message = "Phòng không tồn tại." });
            }

            return Json(new
            {
                success = true,
                room = new
                {
                    soPhong = room.SoPhong,
                    loaiPhong = room.LoaiPhong?.TenLoai,
                    soLuongKhach = room.SoLuongKhach,
                    giaPhong1Dem = room.GiaPhong1Dem,
                    moTa = room.MoTa,
                    hinhAnh = room.HinhAnh,
                    hinhAnh1 = room.HinhAnh1,
                    hinhAnh2 = room.HinhAnh2,
                    hinhAnh3 = room.HinhAnh3
                }
            });
        }
    }
}