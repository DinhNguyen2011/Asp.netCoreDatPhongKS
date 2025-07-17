using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Models.Payment;
using Asp.netCoreDatPhongKS.Models.ViewModels;
using Asp.netCoreDatPhongKS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace Asp.netCoreDatPhongKS.Controllers
{
 
    public class HomeController : Controller
    {
        private readonly HotelPlaceVipContext _context;
        private readonly IVNPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly IEmailService _emailService;

        public HomeController(HotelPlaceVipContext context, IVNPayService vnPayService, IMoMoService moMoService, IEmailService emailService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _emailService = emailService;
        }
  
        private IActionResult RestrictAccessByVaiTro()
        {
            string userName = HttpContext.Session.GetString("Email");

            // Nếu có Hoten trong session, kiểm tra VaiTroId
            if (!string.IsNullOrEmpty(userName))
            {
                // Tìm tài khoản dựa trên Hoten
                var taiKhoan = _context.TaiKhoans
                    .Include(t => t.VaiTro)
                    .FirstOrDefault(t => t.Hoten == userName);

                // Nếu tìm thấy tài khoản và VaiTroId là 1 hoặc 2, từ chối truy cập
                if (taiKhoan != null && (taiKhoan.VaiTroId == 1 || taiKhoan.VaiTroId == 2))
                {
                  //  TempData["Error"] = "Tài khoản admin không được phép truy cập trang này.";
                    return RedirectToAction("Erro", "Home");
                }
            }
            //cho phép truy cập != tk admin
            return null;
        }


        public IActionResult Index()
        {
            var restrictResult = RestrictAccessByVaiTro();
            if (restrictResult != null)
            {
                return restrictResult;
            }
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        [HttpPost]
        public IActionResult TimKiemPhong(DateTime? checkin, DateTime? checkout, int soKhach = 1, int rooms = 1)
        {
            //DateTime checkinDate = checkin ?? DateTime.Today;
            //DateTime checkoutDate = checkout ?? DateTime.Today.AddDays(1);

            //int soDem = (checkoutDate - checkinDate).Days;
            //if (soDem <= 0)
            //{
            //    TempData["ThongBao"] = "Ngày trả phải lớn hơn ngày nhận. Vui lòng kiểm tra lại!";
            //    return View("TimKiemPhong", new List<PhongViewModel>());
            //}

            if (!checkin.HasValue || !checkout.HasValue)
            {
                TempData["ThongBao"] = "Vui lòng chọn cả ngày nhận phòng và ngày trả phòng!";
                return View("Index"); // Trả về view chứa form
            }

            if (rooms < 1 || soKhach < 1)
            {
                TempData["ThongBao"] = "Số phòng và số lượng khách phải lớn hơn hoặc bằng 1!";
                return View("Index"); // Trả về view chứa form
            }

            DateTime checkinDate = checkin.Value;
            DateTime checkoutDate = checkout.Value;

            // Kiểm tra ngày trả có lớn hơn ngày nhận không
            int soDem = (checkoutDate - checkinDate).Days;
            if (soDem <= 0)
            {
                TempData["ThongBao"] = "Ngày trả phòng phải lớn hơn ngày nhận phòng. Vui lòng kiểm tra lại!";
                return View("Index"); // Trả về view chứa form
            }



            var allRooms = _context.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => (p.SoLuongKhach ?? 0) >= soKhach)
                .ToList();

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .Where(p => p.NgayNhan != null && p.NgayTra != null && p.TinhTrangSuDung != "Đã check-out" && p.TinhTrangSuDung !="Đã hủy")
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

            if (!availableRooms.Any())
            {
                TempData["ThongBao"] = "Không tìm thấy phòng khả dụng phù hợp với yêu cầu của bạn. Vui lòng thử lại với khoảng thời gian hoặc số khách khác!";
                return View("Index"); // Trả về view chứa form
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

            //var isRoomBooked = _context.ChiTietPhieuPhongs.Any(c =>
            //    c.PhongId == model.PhongId &&
            //    c.PhieuDatPhong.TinhTrangSuDung != "Đã check-out" && c.PhieuDatPhong.TinhTrangSuDung!= "Đã hủy");
            //((model.Checkin < c.PhieuDatPhong.NgayTra) && (model.Checkout > c.PhieuDatPhong.NgayNhan)));

            //trường hợp này đã so sánh ngày checkin, checkout( bảng tạm), vậy nếu phiếu đã hủy thì sao?? 


            // Kiểm tra phòng có đang được đặt trong khoảng thời gian yêu cầu không
            var bookedRooms = _context.PhieuDatPhongs
        .Include(p => p.ChiTietPhieuPhongs)
        .Where(p => p.TinhTrangSuDung != "Đã check-out" && p.TinhTrangSuDung != "Đã hủy" && p.NgayNhan != null && p.NgayTra != null)
        .SelectMany(p => p.ChiTietPhieuPhongs.Where(c => c.PhongId == model.PhongId)
            .Select(c => new { p.NgayNhan, p.NgayTra }))
        .ToList();

            var isRoomBooked = bookedRooms.Any(booking =>
                !(model.Checkout <= booking.NgayNhan || model.Checkin >= booking.NgayTra));

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

            var paymentMethod = Request.Form["paymentMethod"].ToString();
            if (paymentMethod == "MoMo")
            {
                var paymentUrl = _moMoService.CreatePaymentUrl(paymentModel, HttpContext);
                return Redirect(paymentUrl);
            }
            else
            {
                var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
                return Redirect(paymentUrl);
            }
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
            return await ProcessPaymentCallback(response, "VNPay");
        }

        [HttpGet]
        public async Task<IActionResult> MoMoPaymentCallback()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var response = _moMoService.PaymentExecute(Request.Query);
            return await ProcessPaymentCallback(response, "MoMo");
        }

        private async Task<IActionResult> ProcessPaymentCallback(PaymentResponse response, string paymentMethod)
        {
            var pendingBookingJson = HttpContext.Session.GetString("PendingBooking");
            if (string.IsNullOrEmpty(pendingBookingJson))
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin đặt phòng!";
                return RedirectToAction("Index");
            }

            var pendingBooking = JsonSerializer.Deserialize<PendingBookingModel>(pendingBookingJson);

            var phong = _context.Phongs.Include(p => p.LoaiPhong).FirstOrDefault(p => p.PhongId == pendingBooking.PhongId);
            if (phong == null)
            {
                TempData["ThongBao"] = "Phòng không tồn tại!";
                return RedirectToAction("Index");
            }

            var isRoomBooked = _context.ChiTietPhieuPhongs.Any(c =>
                c.PhongId == pendingBooking.PhongId &&
                c.PhieuDatPhong.TinhTrangSuDung != "Đã check-out" && c.PhieuDatPhong.TinhTrangSuDung != "Đã hủy" &&
                ((pendingBooking.Checkin < c.PhieuDatPhong.NgayTra) 
                && (pendingBooking.Checkout > c.PhieuDatPhong.NgayNhan)));

            if (isRoomBooked)
            {
                TempData["ThongBao"] = "Phòng đã được đặt trong khoảng thời gian này!";
                return RedirectToAction("Index");
            }

            if (response.Success)
            {
                // Kiểm tra Email trong TaiKhoan trước
                var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == pendingBooking.Email);
                if (taiKhoan == null)
                {
                    // Tạo mới TaiKhoan nếu Email chưa tồn tại
                    taiKhoan = new TaiKhoan
                    {
                        Email = pendingBooking.Email,
                        MatKhau = "1", // Mật khẩu mặc định
                        VaiTroId = 3, // Vai trò khách hàng
                        TrangThai = true,
                        Hoten = pendingBooking.HoTen,
                        NgayTao = DateTime.Now,
                        
                    };
                    _context.TaiKhoans.Add(taiKhoan);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra Email trong KhachHangs
                var khach = await _context.KhachHangs.FirstOrDefaultAsync(k => k.Email == pendingBooking.Email);
                if (khach == null)
                {
                    // Tạo mới KhachHang nếu Email chưa tồn tại
                    khach = new KhachHang
                    {
                        HoTen = pendingBooking.HoTen,
                        Email = pendingBooking.Email,
                        SoDienThoai = pendingBooking.SoDienThoai,
                        DiaChi = pendingBooking.DiaChi,
                        Cccd = pendingBooking.Cccd,
                        GhiChu = pendingBooking.GhiChu,
                        NgayTao = DateTime.Now,
                        TaiKhoanId = taiKhoan.TaiKhoanId // Liên kết với TaiKhoan
                    };
                    _context.KhachHangs.Add(khach);
                }
                else
                {
                    // Cập nhật thông tin KhachHang nếu Email đã tồn tại
                    khach.HoTen = pendingBooking.HoTen;
                    khach.SoDienThoai = pendingBooking.SoDienThoai;
                    khach.DiaChi = pendingBooking.DiaChi;
                    khach.Cccd = pendingBooking.Cccd;
                    khach.GhiChu = pendingBooking.GhiChu;
                    khach.TaiKhoanId = taiKhoan.TaiKhoanId; // Liên kết với TaiKhoan
                    _context.Update(khach);
                }

                await _context.SaveChangesAsync();

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
                    VnpTransactionId = paymentMethod == "VNPay" ? response.TransactionId : null,
                    MoMoTransactionId = paymentMethod == "MoMo" ? response.TransactionId : null,
                    SoTienDaThanhToan = pendingBooking.TongTien
                };
                phong.TinhTrang = "Đã đặt";
                _context.PhieuDatPhongs.Add(phieu);

                // Tạo ChiTietPhieuPhong
                var chiTiet = new ChiTietPhieuPhong
                {
                    PhongId = phong.PhongId,
                    DonGia = phong.GiaPhong1Dem
                };
                phieu.ChiTietPhieuPhongs.Add(chiTiet);

                // Tạo HoaDon
                var hoaDon = new HoaDon
                {
                    NgayLap = DateTime.Now,
                    KhachHangId = khach.KhachHangId,
                    NguoiLapDh = "WEBSITE là người lập HĐ",
                    TongTienPhong = pendingBooking.TongTien,
                    TongTienDichVu = 0,
                    TongTien = pendingBooking.TongTien,
                    HinhThucThanhToan = paymentMethod,
                    TrangThai = "Đã thanh toán",
                    IsKhachVangLai = khach.HoTen == "Khách vãng lai",
                    GhiChu = "Hóa đơn phòng đặt qua website"
                };
                _context.HoaDons.Add(hoaDon);

                await _context.SaveChangesAsync();

                // Tạo HoaDonPdp
                var hoaDonPdp = new HoaDonPdp
                {
                    MaHoaDon = hoaDon.MaHoaDon,
                    PhieuDatPhongId = phieu.PhieuDatPhongId,
                    TrangThai = "Đã thanh toán",
                    ThanhTien = phieu.TongTien
                };
                _context.HoaDonPdps.Add(hoaDonPdp);

                await _context.SaveChangesAsync();

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
                            <li><strong>Tổng tiền:</strong> {phieu.TongTien.ToString("N0")} VNĐ</li>
                            <li><strong>Mã giao dịch:</strong> {(paymentMethod == "VNPay" ? phieu.VnpTransactionId : phieu.MoMoTransactionId)}</li>
                            <li><strong>Ghi chú:</strong> {pendingBooking.GhiChu ?? "Không có"}</li>
                        </ul>
                        <p><strong>Thông tin tài khoản:</strong></p>
                        <ul>
                            <li><strong>Email đăng nhập:</strong> {taiKhoan.Email}</li>
                            <li><strong>Mật khẩu mặc định: '1', Vui lòng đăng nhập và đổi mật khẩu nếu lần đăng nhập lần đầu(*Bỏ qua nếu đã có tài khoản*) </strong></li>
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
                TempData["ThongBao"] = $"Thanh toán thất bại! Mã lỗi: {(paymentMethod == "VNPay" ? response.VnPayResponseCode : response.MoMoResponseCode)}";
            }

            HttpContext.Session.Remove("PendingBooking");

            return View("PaymentCallback", response);
        }
    }
}
