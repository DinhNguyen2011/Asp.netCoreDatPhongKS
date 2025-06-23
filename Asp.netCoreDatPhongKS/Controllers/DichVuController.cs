using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class DichVuController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public DichVuController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: DichVu/TrangChuDichVu
        public async Task<IActionResult> TrangChuDichVu(string loaiDichVu)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            // Tạo danh sách các loại dịch vụ cho dropdown
            var serviceTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Tất cả" },
                new SelectListItem { Value = "Đồ ăn", Text = "Đồ ăn" },
                new SelectListItem { Value = "Thể thao", Text = "Thể thao" },
                new SelectListItem { Value = "Hậu cần", Text = "Hậu cần" }
            };
            ViewBag.LoaiDichVu = new SelectList(serviceTypes, "Value", "Text", loaiDichVu);

            // Truy vấn dịch vụ với bộ lọc dựa trên mô tả
            var dichVus = _context.DichVus.AsQueryable();
            if (!string.IsNullOrEmpty(loaiDichVu))
            {
                // Lọc các dịch vụ có MoTa bắt đầu bằng loaiDichVu
                dichVus = dichVus.Where(d => d.MoTa != null && d.MoTa.StartsWith(loaiDichVu));
            }

            // Lấy các dịch vụ đang hoạt động
            var result = await dichVus
                .Where(d => d.TrangThai == true)
                .ToListAsync();

            return View(result);
        }
        // Action Index: Hiển thị danh sách dịch vụ với checkbox
        public async Task<IActionResult> Index(string searchString, string trangThai)
        {
            var userName = HttpContext.Session.GetString("Hoten");
            ViewData["Hoten"] = !string.IsNullOrEmpty(userName) ? userName : "Chưa đăng nhập";

            ViewBag.KhachHangId = new SelectList(
                await _context.KhachHangs
                    .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TinhTrangSuDung == "Đã check-in"))
                    .ToListAsync(),
                "KhachHangId", "HoTen");

            var dichVus = from d in _context.DichVus select d;

            if (!string.IsNullOrEmpty(searchString))
                dichVus = dichVus.Where(d => d.TenDichVu.Contains(searchString));

            if (!string.IsNullOrEmpty(trangThai))
            {
                bool status = trangThai == "Hoạt động";
                dichVus = dichVus.Where(d => d.TrangThai == status);
            }

            return View(await dichVus.ToListAsync());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDonHangDichVu(int? khachHangId, bool isKhachVangLai, int[] dichVuIds, int[] soLuongs, bool thanhToanNgay, string hinhThucThanhToan)
        {
            // Kiểm tra dữ liệu đầu vào
            if (dichVuIds == null || dichVuIds.Length == 0 || soLuongs == null || soLuongs.Length != dichVuIds.Length)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một dịch vụ và số lượng hợp lệ.");
                ViewBag.KhachHangId = new SelectList(
                    await _context.KhachHangs
                        .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TrangThai == "Đang sử dụng"))
                        .ToListAsync(),
                    "KhachHangId", "HoTen", khachHangId);
                return View("Index", await _context.DichVus.ToListAsync());
            }

            if (!isKhachVangLai && !khachHangId.HasValue)
            {
                ModelState.AddModelError("", "Vui lòng chọn khách hàng hoặc chọn khách vãng lai.");
                ViewBag.KhachHangId = new SelectList(
                    await _context.KhachHangs
                        .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TrangThai == "Đang sử dụng"))
                        .ToListAsync(),
                    "KhachHangId", "HoTen", khachHangId);
                return View("Index", await _context.DichVus.ToListAsync());
            }

            if (thanhToanNgay && string.IsNullOrEmpty(hinhThucThanhToan))
            {
                ModelState.AddModelError("", "Vui lòng chọn hình thức thanh toán.");
                ViewBag.KhachHangId = new SelectList(
                    await _context.KhachHangs
                        .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TrangThai == "Đang sử dụng"))
                        .ToListAsync(),
                    "KhachHangId", "HoTen", khachHangId);
                return View("Index", await _context.DichVus.ToListAsync());
            }

            var userName = HttpContext.Session.GetString("Hoten");
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("", "Vui lòng đăng nhập để tạo đơn hàng.");
                ViewBag.KhachHangId = new SelectList(
                    await _context.KhachHangs
                        .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TrangThai == "Đang sử dụng"))
                        .ToListAsync(),
                    "KhachHangId", "HoTen", khachHangId);
                return View("Index", await _context.DichVus.ToListAsync());
            }

            // Tạo đơn hàng dịch vụ
            var donHang = new DonHangDichVu
            {
                KhachHangId = isKhachVangLai ? null : khachHangId,
                NgayDat = DateTime.Now,
                TrangThai = thanhToanNgay || isKhachVangLai ? "Hoàn thành" : "Chờ thanh toán",
                GhiChu = isKhachVangLai ? "Khách vãng lai" : $"Nhân viên lập: {userName}"
            };

            _context.DonHangDichVus.Add(donHang);
            await _context.SaveChangesAsync();

            // Thêm chi tiết dịch vụ
            decimal tongTien = 0;
            for (int i = 0; i < dichVuIds.Length; i++)
            {
                if (soLuongs[i] > 0)
                {
                    var dichVu = await _context.DichVus.FindAsync(dichVuIds[i]);
                    if (dichVu != null)
                    {
                        var chiTiet = new ChiTietDonHangDichVu
                        {
                            MaDonHangDv = donHang.MaDonHangDv,
                            DichVuId = dichVuIds[i],
                            SoLuong = soLuongs[i],
                            DonGia = dichVu.DonGia,
                            ThanhTien = soLuongs[i] * dichVu.DonGia
                        };
                        tongTien += chiTiet.ThanhTien ?? 0;
                        _context.ChiTietDonHangDichVus.Add(chiTiet);
                    }
                }
            }

            if (tongTien == 0)
            {
                ModelState.AddModelError("", "Không có dịch vụ hợp lệ được chọn.");
                ViewBag.KhachHangId = new SelectList(
                    await _context.KhachHangs
                        .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TrangThai == "Đang sử dụng"))
                        .ToListAsync(),
                    "KhachHangId", "HoTen", khachHangId);
                return View("Index", await _context.DichVus.ToListAsync());
            }

            await _context.SaveChangesAsync();

            // Tạo hóa đơn dịch vụ
            var hoaDonDichVu = new HoaDonDichVu
            {
                MaDonHangDv = donHang.MaDonHangDv,
                TrangThaiThanhToan = thanhToanNgay || isKhachVangLai ? "Đã thanh toán" : "Chưa thanh toán",
                NgayThanhToan = thanhToanNgay || isKhachVangLai ? DateTime.Now : null,
                HinhThucThanhToan = thanhToanNgay || isKhachVangLai ? hinhThucThanhToan : null
            };
            _context.HoaDonDichVus.Add(hoaDonDichVu);
            await _context.SaveChangesAsync();

            // Luôn in hóa đơn dịch vụ
            return RedirectToAction("PrintHoaDonDichVu", new { id = hoaDonDichVu.Id });
        }

        // Action PrintHoaDonDichVu: In hóa đơn dịch vụ
        public async Task<IActionResult> PrintHoaDonDichVu(int id)
        {
            var hoaDonDichVu = await _context.HoaDonDichVus
                .Include(h => h.MaDonHangDvNavigation)
                .ThenInclude(d => d.ChiTietDonHangDichVus)
                .ThenInclude(c => c.DichVu)
                .Include(h => h.MaDonHangDvNavigation.KhachHang)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDonDichVu == null)
                return NotFound();

            var userName = HttpContext.Session.GetString("Hoten");
            ViewData["Hoten"] = !string.IsNullOrEmpty(userName) ? userName : "Chưa đăng nhập";

            return View(hoaDonDichVu);
        }
    }
}