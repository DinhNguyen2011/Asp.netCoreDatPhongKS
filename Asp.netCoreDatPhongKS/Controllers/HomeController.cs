using System;
using System.Diagnostics;
using System.Text.Json;
using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Models.Payment;
using Asp.netCoreDatPhongKS.Models.ViewModels;
using Asp.netCoreDatPhongKS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class HomeController : Controller
    {
        private readonly HotelPlaceVipContext _context;
        private readonly IVNPayService _vnPayService;
        private readonly IEmailService _emailService;

        public HomeController(HotelPlaceVipContext context, IVNPayService vnPayService, IEmailService emailService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        [HttpPost]
        public IActionResult TimKiemPhong(DateTime? checkin, DateTime? checkout, int soKhach = 1)
        {
            // Xử lý ngày nhận và ngày trả
            DateTime checkinDate = checkin ?? DateTime.Today;
            DateTime checkoutDate = checkout ?? DateTime.Today.AddDays(1);

            int soDem = (checkoutDate - checkinDate).Days;
            if (soDem <= 0)
            {
                TempData["ThongBao"] = "Ngày trả phải lớn hơn ngày nhận. Vui lòng kiểm tra lại!";
                return View("TimKiemPhong", new List<PhongViewModel>());
            }

            // Tìm kiếm phòng khả dụng
            var allRooms = _context.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => (p.SoLuongKhach ?? 0) >= soKhach) // Đủ sức chứa số khách
                .ToList();

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .Where(p => p.NgayNhan != null && p.NgayTra != null)
                .SelectMany(p => p.ChiTietPhieuPhongs.Select(c => new { c.PhongId, p.NgayNhan, p.NgayTra }))
                .ToList();

            var availableRooms = new List<PhongViewModel>();
            foreach (var room in allRooms)
            {
                bool isAvailable = true;
                foreach (var booking in bookedRooms)
                {
                    if (booking.PhongId == room.PhongId)
                    {
                        DateTime bookingStart = (DateTime)booking.NgayNhan;
                        DateTime bookingEnd = (DateTime)booking.NgayTra;

                        if (!(checkoutDate < bookingStart || checkinDate > bookingEnd))
                        {
                            isAvailable = false;
                            break;
                        }
                    }
                }
                if (isAvailable)
                {
                    availableRooms.Add(new PhongViewModel
                    {
                        Phong = room,
                        Checkin = checkinDate,
                        Checkout = checkoutDate,
                        SoDem = soDem,
                        SoKhach = soKhach
                    });
                }
            }

            // Thông báo nếu không tìm thấy phòng
            if (!availableRooms.Any())
            {
                TempData["ThongBao"] = "Không tìm thấy phòng khả dụng phù hợp với yêu cầu của bạn. Vui lòng thử lại với khoảng thời gian hoặc số khách khác!";
            }

            return View("TimKiemPhong", availableRooms);
        }
        [HttpGet]
        public IActionResult DatPhong(int phongId, DateTime checkin, DateTime checkout)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var phong = _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefault(p => p.PhongId == phongId);

            if (phong == null) return NotFound();

            var vm = new ThongtindatPhongViewModel
            {
                PhongId = phongId,
                Phong = phong,
                Checkin = checkin,
                Checkout = checkout
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DatPhong(ThongtindatPhongViewModel model)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (!ModelState.IsValid)
            {
                model.Phong = _context.Phongs.Include(p => p.LoaiPhong).FirstOrDefault(p => p.PhongId == model.PhongId);
                return View(model);
            }

            var phong = _context.Phongs.FirstOrDefault(p => p.PhongId == model.PhongId);
            if (phong == null)
            {
                ModelState.AddModelError("", "Phòng không tồn tại.");
                return View(model);
            }

            var isRoomBooked = _context.ChiTietPhieuPhongs.Any(c =>
                c.PhongId == model.PhongId &&
                ((model.Checkin < c.PhieuDatPhong.NgayTra) && (model.Checkout > c.PhieuDatPhong.NgayNhan)));

            if (isRoomBooked)
            {
                ModelState.AddModelError("", "Phòng đã được đặt trong khoảng thời gian này.");
                return View(model);
            }

            var khach = _context.KhachHangs.FirstOrDefault(k => k.Cccd == model.Cccd);
            if (khach == null)
            {
                khach = new KhachHang
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    DiaChi = model.DiaChi,
                    Cccd = model.Cccd,
                    GhiChu = model.GhiChu
                };
                _context.KhachHangs.Add(khach);
                _context.SaveChanges();
            }

            int soDem = model.SoDem;
            decimal tongTien = (phong.GiaPhong1Dem ?? 0) * soDem;

            var pendingBooking = new PendingBookingModel
            {
                PhongId = model.PhongId,
                Checkin = model.Checkin,
                Checkout = model.Checkout,
                SoDem = soDem,
                TongTien = tongTien,
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi,
                Cccd = model.Cccd,
                GhiChu = model.GhiChu
            };

            HttpContext.Session.SetString("PendingBooking", JsonSerializer.Serialize(pendingBooking));

            var paymentModel = new PaymentInformationModel
            {
                Amount = tongTien,
                OrderDescription = $"Thanh toan don hang cho phong {phong.SoPhong} tu {model.Checkin:dd/MM/yyyy} den {model.Checkout:dd/MM/yyyy}",
                Name = khach.HoTen,
                PhieuDatPhongId = 0
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
            return Redirect(paymentUrl);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var response = _vnPayService.PaymentExecute(Request.Query);

            var pendingBookingJson = HttpContext.Session.GetString("PendingBooking");
            if (string.IsNullOrEmpty(pendingBookingJson))
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin đặt phòng!";
                return RedirectToAction("Index");
            }

            var pendingBooking = JsonSerializer.Deserialize<PendingBookingModel>(pendingBookingJson);

            var khach = _context.KhachHangs.FirstOrDefault(k => k.Cccd == pendingBooking.Cccd);
            if (khach == null)
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index");
            }

            var phong = _context.Phongs.Include(p => p.LoaiPhong).FirstOrDefault(p => p.PhongId == pendingBooking.PhongId);
            if (phong == null)
            {
                TempData["ThongBao"] = "Phòng không tồn tại!";
                return RedirectToAction("Index");
            }

            var isRoomBooked = _context.ChiTietPhieuPhongs.Any(c =>
                c.PhongId == pendingBooking.PhongId &&
                ((pendingBooking.Checkin < c.PhieuDatPhong.NgayTra) && (pendingBooking.Checkout > c.PhieuDatPhong.NgayNhan)));

            if (isRoomBooked)
            {
                TempData["ThongBao"] = "Phòng đã được đặt trong khoảng thời gian này!";
                return RedirectToAction("Index");
            }

            if (response.Success)
            {
                // Tạo PhieuDatPhong
                var phieu = new PhieuDatPhong
                {
                    MaPhieu = $"PD{DateTime.Now.Ticks}",
                    NgayDat = DateTime.Now,
                    NgayNhan = pendingBooking.Checkin,
                    NgayTra = pendingBooking.Checkout,
                    KhachHangId = khach.KhachHangId,
                    TrangThai = "Đã thanh toán",
                    TinhTrangSuDung = "Chờ xử lý",
                    TongTien = pendingBooking.TongTien,
                    VnpTransactionId = response.TransactionId,
                    SoTienDaThanhToan = pendingBooking.TongTien
                };

                _context.PhieuDatPhongs.Add(phieu);

                // Tạo ChiTietPhieuPhong
                var chiTiet = new ChiTietPhieuPhong
                {
                    PhongId = phong.PhongId,
                    DonGia = phong.GiaPhong1Dem
                };
                phieu.ChiTietPhieuPhongs.Add(chiTiet); // Sử dụng navigation property

                // Tạo HoaDon
                var hoaDon = new HoaDon
                {
                    NgayLap = DateTime.Now,
                    KhachHangId = khach.KhachHangId,
                    NguoiLapDh = "WEBSITE là người lập HĐ",
                    TongTienPhong = pendingBooking.TongTien,
                    TongTienDichVu = 0, // Chưa có dịch vụ
                    TongTien = pendingBooking.TongTien,
                    HinhThucThanhToan = "VNPay",
                    TrangThai = "Đã thanh toán",
                    IsKhachVangLai = khach.HoTen == "Khách vãng lai",
                    GhiChu = "Hóa đơn phòng đặt qua website"

                };
                _context.HoaDons.Add(hoaDon);

               
                await _context.SaveChangesAsync();

                var hoaDonPdp = new HoaDonPdp
                {
                    MaHoaDon = hoaDon.MaHoaDon,
                    PhieuDatPhongId = phieu.PhieuDatPhongId
                };
                _context.HoaDonPdps.Add(hoaDonPdp);

               
                var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Email == khach.Email);
                if (taiKhoan == null)
                {
              
                    taiKhoan = new TaiKhoan
                    {
                        Email = khach.Email,
                        MatKhau = "1",
                        VaiTroId = 3,
                        TrangThai = true,
                        Hoten = khach.HoTen,
                        NgayTao = DateTime.Now
                    };
                    _context.TaiKhoans.Add(taiKhoan);
                }
                

                _context.SaveChanges(); 

              
                if (khach.TaiKhoanId == null)
                {
                    khach.TaiKhoanId = taiKhoan.TaiKhoanId;
                    _context.SaveChanges(); 
                }

                // Gửi email xác nhận
                try
                {
                    var emailBody = $@"
            <h2>Xác nhận đặt phòng và tài khoản thành công</h2>
            <p>Kính gửi {khach.HoTen},</p>
            <p>Cảm ơn bạn đã đặt phòng tại Khách sạn Thiềm Định. Dưới đây là thông tin đặt phòng của bạn:</p>
            <ul>
                <li><strong>Mã phiếu:</strong> {phieu.MaPhieu}</li>
                <li><strong>Phòng:</strong> {phong.SoPhong} ({phong.LoaiPhong?.TenLoai})</li>
                <li><strong>Ngày nhận:</strong> {phieu.NgayNhan?.ToString("dd/MM/yyyy")}</li>
                <li><strong>Ngày trả:</strong> {phieu.NgayTra?.ToString("dd/MM/yyyy")}</li>
                <li><strong>Số đêm:</strong> {pendingBooking.SoDem}</li>
                <li><strong>Tổng tiền:</strong> {phieu.TongTien?.ToString("N0")} VNĐ</li>
                <li><strong>Mã giao dịch:</strong> {phieu.VnpTransactionId}</li>
                <li><strong>Ghi chú:</strong> {pendingBooking.GhiChu ?? "Không có"}</li>
            </ul>
            <p><strong>Thông tin tài khoản:</strong></p>
            <ul>
                <li><strong>Email đăng nhập:</strong> {taiKhoan.Email}</li>
                <li><strong>Mật khẩu mặc định:</strong> {taiKhoan.MatKhau} (Không áp dụng với tài khoản đã có rồi, vui lòng đổi mật khẩu sau khi đăng nhập lần đầu)</li>
            </ul>
            <p>Vui lòng liên hệ chúng tôi nếu có bất kỳ câu hỏi nào. Hotline: 0853461030</p>
            <p>Trân trọng,<br>Khách sạn Thiềm Định</p>";

                    await _emailService.SendEmailAsync(khach.Email, "Xác nhận đặt phòng và tài khoản - Khách sạn Thiềm Định", emailBody);
                }
                catch (Exception ex)
                {
                    TempData["ThongBao"] = $"Đặt phòng và tạo tài khoản thành công, nhưng gửi email thất bại: {ex.Message}";
                }

                TempData["ThongBao"] = $"Thanh toán và đặt phòng thành công! Mã giao dịch: {response.TransactionId}";
            }
            else
            {
                TempData["ThongBao"] = $"Thanh toán thất bại! Mã lỗi: {response.VnPayResponseCode}";
            }

            HttpContext.Session.Remove("PendingBooking");

            return View("PaymentCallback", response);
        }
    }
}