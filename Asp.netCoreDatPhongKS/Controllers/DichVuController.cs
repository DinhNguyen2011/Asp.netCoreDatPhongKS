//using Asp.netCoreDatPhongKS.Models;
//using Asp.netCoreDatPhongKS.Models.ViewModels; // Thêm dòng này để import namespace
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Asp.netCoreDatPhongKS.Controllers
//{
//    public class DichVuController : Controller
//    {
//        //đặt dịch vụ, hd liên kết vs pđp lấy tt tổng hợp,
//        //pđp liên kết khách hàng lấy tt KH
//        //kl với chi tiet phieuphong và chitietdichvu để lấy chi tiết phòng và dịch vụ
//        //ctpp lấy số phòng 
//        // ct dịch vụ lấy dịch vụ và giá
//        private readonly HotelPlaceVipContext _context;

//        public DichVuController(HotelPlaceVipContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index(string loaiDichVu)
//        {
//            string userName = HttpContext.Session.GetString("Hoten");
//            if (!string.IsNullOrEmpty(userName))
//            {
//                ViewData["Hoten"] = userName;
//            }
//            var dichVus = from dv in _context.DichVus
//                          where dv.TrangThai == true // Chỉ lấy dịch vụ đang hoạt động
//                          select dv;

          
//            if (!string.IsNullOrEmpty(loaiDichVu) && loaiDichVu != "TatCa")
//            {
//                dichVus = dichVus.Where(dv => dv.MoTa != null && dv.MoTa.Contains(loaiDichVu));
//            }

//            // Lấy danh sách loại dịch vụ (dựa trên MoTa)
//            var loaiDichVus = new List<string> { "TatCa", "Thể thao", "Đồ ăn", "Hậu cần","Thức uống" };

//            // Tạo ViewModel
//            var viewModel = new DichVuViewModel
//            {
//                DichVus = await dichVus.ToListAsync(),
//                LoaiDichVus = loaiDichVus,
//                LoaiDichVuHienTai = loaiDichVu ?? "TatCa"
//            };

//            return View(viewModel);
//        }
//        [HttpPost]
//        public async Task<IActionResult> DatDichVu(int DichVuId, int SoLuong)
//        {
//            string userName = HttpContext.Session.GetString("Hoten");
//            if (!string.IsNullOrEmpty(userName))
//            {
//                ViewData["Hoten"] = userName;
//            }
//            // Giả định khách đã đăng nhập, lấy PhieuDatPhong từ Session
//            var phieuDatPhongId = HttpContext.Session.GetInt32("PhieuDatPhongId");
//            if (phieuDatPhongId == null)
//            {
//                return RedirectToAction("Login", "TaiKhoan");
//            }

//            var phieuDatPhong = await _context.PhieuDatPhongs
//                .Include(p => p.ChiTietPhieuPhongs)
//                .ThenInclude(ctp => ctp.Phong)
//                .Include(p => p.ChiTietDichVus)
//                .ThenInclude(ctdv => ctdv.DichVu)
//                .Include(p => p.KhachHang)
//                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == phieuDatPhongId);

//            if (phieuDatPhong == null)
//            {
//                return NotFound("Không tìm thấy phiếu đặt phòng.");
//            }

//            // Thêm chi tiết dịch vụ
//            var dichVu = await _context.DichVus.FindAsync(DichVuId);
//            if (dichVu == null)
//            {
//                return NotFound("Không tìm thấy dịch vụ.");
//            }

//            var chiTietDichVu = new ChiTietDichVu
//            {
//                PhieuDatPhongId = phieuDatPhong.PhieuDatPhongId,
//                DichVuId = DichVuId,
//                SoLuong = SoLuong,
//                DonGia = dichVu.DonGia
//            };
//            _context.ChiTietDichVus.Add(chiTietDichVu);
//            await _context.SaveChangesAsync();

//            // Cập nhật TongTien trong PhieuDatPhong (chỉ tính chi phí dịch vụ)
//            var tongTienDichVu = phieuDatPhong.ChiTietDichVus?.Sum(ctdv => ctdv.SoLuong * ctdv.DonGia) ?? 0m; // Bỏ ?? trong phép tính vì DonGia là decimal
//            phieuDatPhong.TongTien = tongTienDichVu; // Chỉ cập nhật chi phí dịch vụ
//            await _context.SaveChangesAsync();

//            // Tạo hoặc cập nhật HoaDon
//            var hoaDon = await _context.HoaDons
//                .FirstOrDefaultAsync(hd => hd.DonHangDichVus == phieuDatPhong.PhieuDatPhongId);
//            var tongTienPhong = phieuDatPhong.ChiTietPhieuPhongs?.Sum(ctp => ctp.DonGia) ?? 0m; // Bỏ ?? trong phép tính vì DonGia là decimal
//            if (hoaDon == null)
//            {
//                hoaDon = new HoaDon
//                {
//                   // PhieuDatPhongId = phieuDatPhong.PhieuDatPhongId,
//                    TongTienPhong = tongTienPhong,
//                    TongTienDichVu = tongTienDichVu,
//                    GiamGia = 0m,
//                    TongTien = tongTienPhong + tongTienDichVu,
//                    NgayLap = DateTime.Now,
//                    TrangThai = "Chưa thanh toán",
//                    PhuongThucThanhToan = "Tiền mặt"
//                };
//                _context.HoaDons.Add(hoaDon);
//            }
//            else
//            {
//                hoaDon.TongTienPhong = tongTienPhong;
//                hoaDon.TongTienDichVu = tongTienDichVu;
//                hoaDon.TongTien = tongTienPhong + tongTienDichVu;
//            }
//            await _context.SaveChangesAsync();

//            return RedirectToAction("XemHoaDon", new { hoaDonId = hoaDon.HoaDonId });
//        }

//        public async Task<IActionResult> XemHoaDon(int hoaDonId)
//        {
//            var hoaDon = await (from hd in _context.HoaDons
//                                join pdp in _context.PhieuDatPhongs on hd.PhieuDatPhongId equals pdp.PhieuDatPhongId
//                                join kh in _context.KhachHangs on pdp.KhachHangId equals kh.KhachHangId
//                                join ctp in _context.ChiTietPhieuPhongs on pdp.PhieuDatPhongId equals ctp.PhieuDatPhongId
//                                join p in _context.Phongs on ctp.PhongId equals p.PhongId
//                                join ctdv in _context.ChiTietDichVus on pdp.PhieuDatPhongId equals ctdv.PhieuDatPhongId
//                                join dv in _context.DichVus on ctdv.DichVuId equals dv.DichVuId
//                                where hd.HoaDonId == hoaDonId
//                                select new
//                                {
//                                    TenKhachHang = kh.HoTen,
//                                    SoPhong = p.SoPhong,
//                                    SoDienThoai = kh.SoDienThoai,
//                                    TenDichVu = dv.TenDichVu,
//                                    SoLuong = ctdv.SoLuong,
//                                    DonGiaDichVu = ctdv.DonGia, // Bỏ ?? vì DonGia là decimal
//                                    TongTienPhong = hd.TongTienPhong,
//                                    TongTienDichVu = hd.TongTienDichVu,
//                                    GiamGia = hd.GiamGia,
//                                    TongTien = hd.TongTien,
//                                    NgayLap = hd.NgayLap,
//                                    PhuongThucThanhToan = hd.PhuongThucThanhToan
//                                }).ToListAsync();

//            if (hoaDon == null || !hoaDon.Any())
//            {
//                return NotFound("Không tìm thấy hóa đơn.");
//            }

//            ViewBag.HoaDon = hoaDon;
//            return View();
//        }
//    }
//}