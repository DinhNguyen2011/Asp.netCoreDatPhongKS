using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        [RestrictToAdmin]
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

        [RestrictToAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDonHangDichVu(int? khachHangId, bool isKhachVangLai, int[] dichVuIds, int[] soLuongs, bool thanhToanNgay, string hinhThucThanhToan)
        {
         
            // Khôi phục ViewBag.KhachHangId
            ViewBag.KhachHangId = new SelectList(
                await _context.KhachHangs
                    .Where(kh => _context.PhieuDatPhongs.Any(p => p.KhachHangId == kh.KhachHangId && p.TinhTrangSuDung == "Đã check-in"))
                    .ToListAsync(),
                "KhachHangId", "HoTen", khachHangId);

            // Kiểm tra dữ liệu đầu vào
            if (dichVuIds == null || dichVuIds.Length == 0 || soLuongs == null || soLuongs.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một dịch vụ và số lượng hợp lệ.";
                return View("Index", await _context.DichVus.ToListAsync());
            }

            // Lọc các dịch vụ có số lượng > 0
            var validDichVus = dichVuIds.Zip(soLuongs, (id, sl) => new { DichVuId = id, SoLuong = sl })
                .Where(x => x.SoLuong > 0)
                .ToList();

            if (!validDichVus.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một dịch vụ với số lượng lớn hơn 0.";
                return View("Index", await _context.DichVus.ToListAsync());
            }

            // Validation khách hàng
            if (!isKhachVangLai && !khachHangId.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn khách hàng hoặc chọn khách vãng lai.";
                return View("Index", await _context.DichVus.ToListAsync());
            }

            // Validation hình thức thanh toán
            if ((thanhToanNgay || isKhachVangLai) && string.IsNullOrEmpty(hinhThucThanhToan))
            {
                TempData["Error"] = "Vui lòng chọn hình thức thanh toán.";
                return View("Index", await _context.DichVus.ToListAsync());
            }

            // Lấy thông tin nhân viên
            var userName = HttpContext.Session.GetString("Hoten") ?? "Nhân viên không xác định";

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
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
                    foreach (var item in validDichVus)
                    {
                        var dichVu = await _context.DichVus.FindAsync(item.DichVuId);
                        if (dichVu != null)
                        {
                            var chiTiet = new ChiTietDonHangDichVu
                            {
                                MaDonHangDv = donHang.MaDonHangDv,
                                DichVuId = item.DichVuId,
                                SoLuong = item.SoLuong,
                                DonGia = dichVu.DonGia,
                                ThanhTien = item.SoLuong * dichVu.DonGia
                            };
                            tongTien += chiTiet.ThanhTien ?? 0;
                            _context.ChiTietDonHangDichVus.Add(chiTiet);
                        }
                    }

                    if (tongTien == 0)
                    {
                        throw new Exception("Không có dịch vụ hợp lệ được chọn.");
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

                    // Tạo hóa đơn tổng (HoaDon) nếu cần
                    var hoaDon = new HoaDon
                    {
                        NgayLap = DateTime.Now,
                        KhachHangId = isKhachVangLai ? null : khachHangId,
                        TongTienPhong = 0, // Không có phòng trong trường hợp này
                        TongTienDichVu = tongTien,
                        TongTien = tongTien,
                        HinhThucThanhToan = hinhThucThanhToan,
                        TrangThai = thanhToanNgay || isKhachVangLai ? "Đã thanh toán" : "Chưa thanh toán",
                        IsKhachVangLai = isKhachVangLai,
                        GhiChu = isKhachVangLai ? "Hóa đơn dịch vụ cho khách vãng lai" : $"Hóa đơn dịch vụ, lập bởi: {userName}",
                        SoTienConNo = thanhToanNgay || isKhachVangLai ? 0 : tongTien,
                        NguoiLapDh = userName
                    };
                    _context.HoaDons.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    // Liên kết HoaDonDichVu với HoaDon
                    hoaDonDichVu.MaHoaDonTong = hoaDon.MaHoaDon;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["Success"] = "Tạo đơn hàng dịch vụ thành công.";
                    return RedirectToAction("PrintHoaDonDichVu", new { id = hoaDonDichVu.Id });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo đơn hàng: {ex.Message}");
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return View("Index", await _context.DichVus.ToListAsync());
            }
        }

        [RestrictToAdmin]
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
        [RestrictToAdmin]
        public IActionResult IndexQLDichVu()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var dv = _context.DichVus.ToList();
            return View(dv);
        }
        [RestrictToAdmin]

        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        // POST: DichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DichVu model, IFormFile? HinhAnhFile)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (string.IsNullOrEmpty(model.TenDichVu) || model.DonGia <= 0)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ Tên dịch vụ và Đơn giá hợp lệ.";
                return View(model);
            }

            try
            {
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/dichvu", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnhFile.CopyToAsync(stream);
                    }
                    model.HinhAnh = "/images/dichvu/" + fileName;
                }

                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                model.TrangThai ??= true;

                _context.DichVus.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm dịch vụ thành công.";
                return RedirectToAction(nameof(IndexQLDichVu));
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm dịch vụ.";
                return View(model);
            }
        }

        [RestrictToAdmin]

        // GET: DichVu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null)
                return NotFound();

            return View(dichVu);
        }

        // POST: DichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DichVu model, IFormFile? HinhAnhFile)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id != model.DichVuId)
                return NotFound();

            if (string.IsNullOrEmpty(model.TenDichVu) || model.DonGia <= 0)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ Tên dịch vụ và Đơn giá hợp lệ.";
                return View(model);
            }

            var existingDichVu = await _context.DichVus.FindAsync(id);
            if (existingDichVu == null)
                return NotFound();

            try
            {
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/dichvu", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnhFile.CopyToAsync(stream);
                    }
                    // Xóa hình cũ nếu có
                    if (!string.IsNullOrEmpty(existingDichVu.HinhAnh))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingDichVu.HinhAnh.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }
                    existingDichVu.HinhAnh = "/images/dichvu/" + fileName;
                }

                existingDichVu.TenDichVu = model.TenDichVu;
                existingDichVu.MoTa = model.MoTa;
                existingDichVu.DonGia = model.DonGia;
                existingDichVu.TrangThai = model.TrangThai ?? true;
                existingDichVu.NgayCapNhat = DateTime.Now;

                _context.Update(existingDichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sửa dịch vụ thành công.";
                return RedirectToAction(nameof(IndexQLDichVu));
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi sửa dịch vụ.";
                return View(model);
            }
        }

        [RestrictToAdmin]

        // GET: DichVu/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null)
                return NotFound();

            return View(dichVu);
        }
        [RestrictToAdmin]

        // POST: DichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null)
                return NotFound();

            try
            {
                if (!string.IsNullOrEmpty(dichVu.HinhAnh))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", dichVu.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa dịch vụ thành công.";
                return RedirectToAction(nameof(IndexQLDichVu));
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa dịch vụ.";
                return View(dichVu);
            }
        }
    }
}