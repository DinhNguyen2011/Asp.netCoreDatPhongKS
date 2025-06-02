using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public CheckOutController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var phieuDatPhongs = _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Where(p => p.TinhTrangSuDung == "Đã check-in")
                .ToList();

            return View(phieuDatPhongs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckOut(int phieuDatPhongId, string hinhThucThanhToan)
        {
            var phieu = _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .FirstOrDefault(p => p.PhieuDatPhongId == phieuDatPhongId);

            if (phieu == null || phieu.TinhTrangSuDung != "Đã check-in")
            {
                TempData["ThongBao"] = "Phiếu đặt phòng không hợp lệ hoặc khách chưa check-in!";
                return RedirectToAction("Index");
            }

            // Tìm tất cả DonHangDichVu chưa thanh toán của khách
            var donHangDichVus = _context.DonHangDichVus
                .Include(d => d.ChiTietDonHangDichVus)
                .ThenInclude(c => c.DichVu)
                .Where(d => d.KhachHangId == phieu.KhachHangId && d.TrangThai == "Chưa thanh toán")
                .ToList();

            // Tính tổng tiền phòng
            decimal tongTienPhong = 0;
            foreach (var chiTiet in phieu.ChiTietPhieuPhongs)
            {
                int soDem = (phieu.NgayTra.Value - phieu.NgayNhan.Value).Days;
                soDem = soDem > 0 ? soDem : 1;
                tongTienPhong += chiTiet.Phong.GiaPhong1Dem * soDem ?? 0;
            }

            // Tính tổng tiền dịch vụ
            decimal tongTienDichVu = 0;
            foreach (var donHang in donHangDichVus)
            {
                foreach (var chiTiet in donHang.ChiTietDonHangDichVus)
                {
                    tongTienDichVu += chiTiet.SoLuong * chiTiet.DonGia;
                }
            }

            // Tạo HoaDon
            var hoaDon = new HoaDon
            {
                NgayLap = new DateTime(2025, 6, 2, 0, 36, 0, DateTimeKind.Local), // 12:36 AM +07, 02/06/2025
                KhachHangId = phieu.KhachHangId,
                NhanVienId = 1, // Giả định, có thể lấy từ session
                TongTienPhong = tongTienPhong,
                TongTienDichVu = tongTienDichVu,
                TongTien = tongTienPhong + tongTienDichVu,
                HinhThucThanhToan = hinhThucThanhToan,
                TrangThai = "Đã thanh toán",
                IsKhachVangLai = phieu.KhachHang?.HoTen == "Khách vãng lai"
            };
            _context.HoaDons.Add(hoaDon);

            // Liên kết HoaDon với PhieuDatPhong qua HoaDonPdp
            var hoaDonPdp = new HoaDonPdp
            {
                MaHoaDon = hoaDon.MaHoaDon,
                PhieuDatPhongId = phieu.PhieuDatPhongId
            };
            _context.HoaDonPdps.Add(hoaDonPdp);

            // Liên kết HoaDon với DonHangDichVu qua HoaDonDichVu
            foreach (var donHang in donHangDichVus)
            {
                var hoaDonDichVu = new HoaDonDichVu
                {
                    MaHoaDonTong = hoaDon.MaHoaDon,
                    MaDonHangDv = donHang.MaDonHangDv
                };
                _context.HoaDonDichVus.Add(hoaDonDichVu);
                donHang.TrangThai = "Đã thanh toán";
            }

            // Tạo ChiTietHoaDon
            foreach (var chiTiet in phieu.ChiTietPhieuPhongs)
            {
                int soDem = (phieu.NgayTra.Value - phieu.NgayNhan.Value).Days;
                soDem = soDem > 0 ? soDem : 1;
                decimal thanhTien = chiTiet.Phong.GiaPhong1Dem * soDem?? 0;
                var chiTietHoaDon = new ChiTietHoaDon
                {
                    MaHoaDon = hoaDon.MaHoaDon,
                    PhongId = chiTiet.PhongId,
                    DichVuId = null,
                    MoTa = $"Phòng {chiTiet.Phong.SoPhong} - {soDem} ngày",
                    SoLuong = soDem,
                    DonGia = chiTiet.Phong.GiaPhong1Dem ?? 0,
                    ThanhTien = thanhTien
                };
                _context.ChiTietHoaDons.Add(chiTietHoaDon);
            }

            foreach (var donHang in donHangDichVus)
            {
                foreach (var chiTiet in donHang.ChiTietDonHangDichVus)
                {
                    decimal thanhTien = chiTiet.SoLuong * chiTiet.DonGia;
                    var chiTietHoaDon = new ChiTietHoaDon
                    {
                        MaHoaDon = hoaDon.MaHoaDon,
                        PhongId = null,
                        DichVuId = chiTiet.DichVuId,
                        MoTa = $"{chiTiet.DichVu.TenDichVu} - {chiTiet.SoLuong} suất",
                        SoLuong = chiTiet.SoLuong,
                        DonGia = chiTiet.DonGia,
                        ThanhTien = thanhTien
                    };
                    _context.ChiTietHoaDons.Add(chiTietHoaDon);
                }
            }

            // Cập nhật trạng thái PhieuDatPhong
            phieu.TinhTrangSuDung = "Đã check-out";

            await _context.SaveChangesAsync();

            return RedirectToAction("InHoaDon", new { hoaDonId = hoaDon.MaHoaDon });
        }

        [HttpGet]
        public IActionResult InHoaDon(int hoaDonId)
        {
            var hoaDon = _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(c => c.Phong)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(c => c.DichVu)
                .FirstOrDefault(h => h.MaHoaDon == hoaDonId);

            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index");
            }

            return View(hoaDon);
        }
    }
}