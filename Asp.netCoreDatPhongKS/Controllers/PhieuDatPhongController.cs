using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class PhieuDatPhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public PhieuDatPhongController(HotelPlaceVipContext context)
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


        public IActionResult hienThiphieuHuy()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var huypdp = _context.PhieuDatPhongs
                          .Include(p => p.KhachHang)
                   .Where(p => p.TrangThai == "Hủy")
                   .ToList();
            return View(huypdp);
        }


        [AuthorizePermission("ManagePhieuDatPhong")]
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.Phongs = GetAvailableRooms(null, null, null);
            ViewBag.TrangThaiOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Chưa thanh toán", Text = "Chưa thanh toán" },
                new SelectListItem { Value = "Đã thanh toán", Text = "Đã thanh toán" },
                new SelectListItem { Value = "Hủy", Text = "Hủy" },
            };
               // new SelectListItem { Value = "Hoàn thành", Text = "Hoàn thành" }
            ViewBag.TinhTrangSuDungOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Chưa sử dụng", Text = "Chưa sử dụng" },
                new SelectListItem { Value = "Đã check-in", Text = "Đã check-in" },
                new SelectListItem { Value = "Đã check-out", Text = "Đã check-out" },
                new SelectListItem { Value = "Chờ xử lý", Text = "Chờ xử lý" }
            };
            ViewBag.KhuyenMaiOptions = _context.KhuyenMais
                .Where(km => km.TrangThai == true && km.NgayBatDau <= DateTime.Now && km.NgayKetThuc >= DateTime.Now)
                .Select(km => new SelectListItem
                {
                    Value = km.KhuyenMaiId.ToString(),
                    Text = $"{km.MaKhuyenMai} - {km.MoTa} ({km.PhanTramGiam}% - {km.NgayBatDau:dd/MM/yyyy} đến {km.NgayKetThuc:dd/MM/yyyy})"
                }).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> Create(PhieuDatPhong model, List<int> phongIds, decimal soTienCoc, decimal? soTienDaThanhToan, string trangThai, string tinhTrangSuDung, int? khuyenMaiId)
        {
            var validTrangThai = new List<string> { "Chưa thanh toán", "Đã thanh toán", "Hủy", "Hoàn thành" };
           // var validTrangThai = new List<string> { "Chưa thanh toán", "Đã thanh toán", "Hủy", "Hoàn thành" };

            var validTinhTrangSuDung = new List<string> { "Chưa sử dụng", "Đã check-in", "Đã check-out", "Chờ xử lý" };

            // Validation
            if (!ModelState.IsValid || phongIds == null || !phongIds.Any())
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin và chọn ít nhất một phòng.";
            }
            if (soTienCoc < 0)
            {
                ModelState.AddModelError("soTienCoc", "Tiền cọc không được âm.");
            }
            if (soTienDaThanhToan.HasValue && soTienDaThanhToan < 0)
            {
                ModelState.AddModelError("soTienDaThanhToan", "Số tiền đã thanh toán không được âm.");
            }
            if (!string.IsNullOrEmpty(trangThai) && !validTrangThai.Contains(trangThai))
            {
                ModelState.AddModelError("TrangThai", "Trạng thái không hợp lệ.");
            }
            if (!string.IsNullOrEmpty(tinhTrangSuDung) && !validTinhTrangSuDung.Contains(tinhTrangSuDung))
            {
                ModelState.AddModelError("TinhTrangSuDung", "Tình trạng sử dụng không hợp lệ.");
            }
            if (model.NgayNhan >= model.NgayTra)
            {
                ModelState.AddModelError("NgayNhan", "Ngày nhận phải trước ngày trả.");
            }
            if (khuyenMaiId.HasValue)
            {
                var khuyenMai = await _context.KhuyenMais.FindAsync(khuyenMaiId);
                if (khuyenMai == null || khuyenMai.TrangThai != true || khuyenMai.NgayBatDau > DateTime.Now || khuyenMai.NgayKetThuc < DateTime.Now)
                {
                    ModelState.AddModelError("KhuyenMaiId", "Khuyến mãi không hợp lệ hoặc đã hết hạn.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.KhachHangs = _context.KhachHangs.ToList();
                ViewBag.Phongs = GetAvailableRooms(model.NgayNhan, model.NgayTra, null);
                ViewBag.TrangThaiOptions = validTrangThai.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
                ViewBag.TinhTrangSuDungOptions = validTinhTrangSuDung.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
                ViewBag.KhuyenMaiOptions = _context.KhuyenMais
                    .Where(km => km.TrangThai == true && km.NgayBatDau <= DateTime.Now && km.NgayKetThuc >= DateTime.Now)
                    .Select(km => new SelectListItem
                    {
                        Value = km.KhuyenMaiId.ToString(),
                        Text = $"{km.MaKhuyenMai} - {km.MoTa} ({km.PhanTramGiam}% - {km.NgayBatDau:dd/MM/yyyy} đến {km.NgayKetThuc:dd/MM/yyyy})"
                    }).ToList();
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
                    soNgay = soNgay < 1 ? 1 : soNgay;
                    var tongTien = donGia * soNgay;

                    // Áp dụng khuyến mãi
                    if (khuyenMaiId.HasValue)
                    {
                        var khuyenMai = await _context.KhuyenMais.FindAsync(khuyenMaiId);
                        if (khuyenMai != null)
                        {
                            var phanTramGiam = khuyenMai.PhanTramGiam ?? 0;
                            tongTien *= (1 - phanTramGiam / 100.0m);
                        }
                    }

                    var soTienDaThanhToanPerRoom = soTienDaThanhToan.HasValue ? soTienDaThanhToan.Value / phongIds.Count : 0;
                    if (soTienDaThanhToanPerRoom > tongTien)
                    {
                        TempData["Error"] = $"Số tiền đã thanh toán cho phòng {phong.SoPhong} vượt quá tổng tiền.";
                        throw new Exception($"Số tiền đã thanh toán vượt quá tổng tiền.");
                    }

                    var phieu = new PhieuDatPhong
                    {
                        MaPhieu = $"PDP-{DateTime.Now:yyyyMMddHHmmss}-{phongId}",
                        KhachHangId = model.KhachHangId,
                        NgayDat = DateTime.Now,
                        NgayNhan = model.NgayNhan,
                        NgayTra = model.NgayTra,
                        TongTien = tongTien,
                        SoTienCoc = soTienCoc / phongIds.Count,
                        SoTienDaThanhToan = soTienDaThanhToanPerRoom,
                        TrangThai = soTienDaThanhToanPerRoom >= tongTien ? "Đã thanh toán" : (string.IsNullOrEmpty(trangThai) ? "Chưa thanh toán" : trangThai),
                        TinhTrangSuDung = string.IsNullOrEmpty(tinhTrangSuDung) ? "Chưa sử dụng" : tinhTrangSuDung,
                        KhuyenMaiId = khuyenMaiId,
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
                ViewBag.TrangThaiOptions = validTrangThai.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
                ViewBag.TinhTrangSuDungOptions = validTinhTrangSuDung.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
                ViewBag.KhuyenMaiOptions = _context.KhuyenMais
                    .Where(km => km.TrangThai == true && km.NgayBatDau <= DateTime.Now && km.NgayKetThuc >= DateTime.Now)
                    .Select(km => new SelectListItem
                    {
                        Value = km.KhuyenMaiId.ToString(),
                        Text = $"{km.MaKhuyenMai} - {km.MoTa} ({km.PhanTramGiam}% - {km.NgayBatDau:dd/MM/yyyy} đến {km.NgayKetThuc:dd/MM/yyyy})"
                    }).ToList();
                return View(model);
            }
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
                .Where(p => p.NgayNhan != null && p.NgayTra != null && p.TrangThai != "Hủy" && p.TinhTrangSuDung !="Đã check-out")
               // .Where(p => p.NgayNhan != null && p.NgayTra != null && p.TrangThai != "Hủy" && p.TrangThai != "Hoàn thành" && p.TinhTrangSuDung !="Đã check-out")
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
        private bool IsRoomAvailable(int phongId, DateTime? newNgayNhan, DateTime? newNgayTra)
        {
            if (!newNgayNhan.HasValue || !newNgayTra.HasValue) return false;

            var bookedRooms = _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .Where(p => p.NgayNhan != null && p.NgayTra != null && p.TrangThai != "Hủy" && p.TrangThai != "Hoàn thành" && p.TinhTrangSuDung != "Đã check-out")
                .SelectMany(p => p.ChiTietPhieuPhongs.Where(c => c.PhongId == phongId)
                    .Select(c => new { p.NgayNhan, p.NgayTra }))
                .ToList();

            return !bookedRooms.Any(booking =>
                !(newNgayTra.Value <= booking.NgayNhan || newNgayNhan.Value >= booking.NgayTra));
        }

        // GET: Hiển thị form chỉnh sửa phiếu đặt phòng
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> Edit(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var phieu = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .ThenInclude(p => p!.LoaiPhong)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == id);

            if (phieu == null)
            {
                TempData["Error"] = "Phiếu đặt phòng không tồn tại.";
                return RedirectToAction("Index");
            }

            if (phieu.TinhTrangSuDung == "Đã check-out")
            {
                TempData["Error"] = "Không thể sửa phiếu đã check-out.";
                return RedirectToAction("Index");
            }

            ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
            ViewBag.TrangThaiOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Chưa thanh toán", Text = "Chưa thanh toán" },
                new SelectListItem { Value = "Đã thanh toán", Text = "Đã thanh toán" },
                new SelectListItem { Value = "Hủy", Text = "Hủy" },
               // new SelectListItem { Value = "Hoàn thành", Text = "Hoàn thành" }
            };
            ViewBag.TinhTrangSuDungOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Chưa sử dụng", Text = "Chưa sử dụng" },
                new SelectListItem { Value = "Đã check-in", Text = "Đã check-in" },
                new SelectListItem { Value = "Đã check-out", Text = "Đã check-out" },
                new SelectListItem { Value = "Chờ xử lý", Text = "Chờ xử lý" }
            };
            return View(phieu);
        }

        // POST: Xử lý cập nhật phiếu đặt phòng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> Edit(int id, PhieuDatPhong model, decimal? soTienCoc, decimal? soTienDaThanhToan)
        {
            if (id != model.PhieuDatPhongId)
            {
                TempData["Error"] = "ID phiếu không khớp.";
                return RedirectToAction("Index");
            }

            var phieu = await _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .Include(p => p.KhuyenMai)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == id);

            if (phieu == null)
            {
                TempData["Error"] = "Phiếu đặt phòng không tồn tại.";
                return RedirectToAction("Index");
            }

            if (phieu.TinhTrangSuDung == "Đã check-out")
            {
                TempData["Error"] = "Không thể sửa phiếu đã check-out.";
                return RedirectToAction("Index");
            }

            // Validation cho SoTienCoc và SoTienDaThanhToan
            if (soTienCoc.HasValue && soTienCoc < 0)
            {
                ModelState.AddModelError("SoTienCoc", "Tiền cọc không được âm.");
            }
            if (soTienDaThanhToan.HasValue && soTienDaThanhToan < 0)
            {
                ModelState.AddModelError("SoTienDaThanhToan", "Số tiền đã thanh toán không được âm.");
            }
            if (soTienDaThanhToan.HasValue && soTienDaThanhToan > (phieu.TongTien ?? 0))
            {
                ModelState.AddModelError("SoTienDaThanhToan", "Số tiền đã thanh toán không được vượt quá tổng tiền.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra thông tin nhập.";
                ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
                ViewBag.TrangThaiOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Chưa thanh toán", Text = "Chưa thanh toán" },
                    new SelectListItem { Value = "Đã thanh toán", Text = "Đã thanh toán" },
                    new SelectListItem { Value = "Hủy", Text = "Hủy" },
                 //   new SelectListItem { Value = "Hoàn thành", Text = "Hoàn thành" }
                };
                ViewBag.TinhTrangSuDungOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Chưa sử dụng", Text = "Chưa sử dụng" },
                    new SelectListItem { Value = "Đã check-in", Text = "Đã check-in" },
                    new SelectListItem { Value = "Đã check-out", Text = "Đã check-out" },
                    new SelectListItem { Value = "Chờ xử lý", Text = "Chờ xử lý" }
                };
                return View(model);
            }

            IDbContextTransaction transaction = null;
            try
            {
                transaction = await _context.Database.BeginTransactionAsync();

                // Cập nhật các trường
                phieu.KhachHangId = model.KhachHangId > 0 ? model.KhachHangId : phieu.KhachHangId;
                phieu.NgayNhan = model.NgayNhan ?? phieu.NgayNhan;
                phieu.NgayTra = model.NgayTra ?? phieu.NgayTra;
                phieu.TrangThai = !string.IsNullOrEmpty(model.TrangThai) ? model.TrangThai : phieu.TrangThai;
                phieu.TinhTrangSuDung = !string.IsNullOrEmpty(model.TinhTrangSuDung) ? model.TinhTrangSuDung : phieu.TinhTrangSuDung;
                phieu.SoTienCoc = soTienCoc ?? phieu.SoTienCoc;
                phieu.SoTienDaThanhToan = soTienDaThanhToan ?? phieu.SoTienDaThanhToan;

                // Tính lại TongTien nếu NgayNhan hoặc NgayTra thay đổi
                if (model.NgayNhan != phieu.NgayNhan || model.NgayTra != phieu.NgayTra)
                {
                    decimal tongTien = 0;
                    var soNgay = ((model.NgayTra ?? phieu.NgayTra) - (model.NgayNhan ?? phieu.NgayNhan))?.Days ?? 1;
                    soNgay = soNgay < 1 ? 1 : soNgay;

                    foreach (var chiTiet in phieu.ChiTietPhieuPhongs)
                    {
                        var donGia = chiTiet.DonGia ?? (chiTiet.Phong?.GiaPhong1Dem ?? 0);
                        tongTien += donGia * soNgay;
                    }

                    if (phieu.KhuyenMaiId.HasValue && phieu.KhuyenMai != null)
                    {
                        var phanTramGiam = phieu.KhuyenMai.PhanTramGiam ?? 0;
                        tongTien *= (1 - phanTramGiam / 100);
                    }

                    phieu.TongTien = tongTien;
                }

                _context.PhieuDatPhongs.Update(phieu);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Cập nhật phiếu đặt phòng thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                ViewBag.KhachHangs = await _context.KhachHangs.ToListAsync();
                ViewBag.TrangThaiOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Chưa thanh toán", Text = "Chưa thanh toán" },
                    new SelectListItem { Value = "Đã thanh toán", Text = "Đã thanh toán" },
                    new SelectListItem { Value = "Hủy", Text = "Hủy" },
                  //  new SelectListItem { Value = "Hoàn thành", Text = "Hoàn thành" }
                };
                ViewBag.TinhTrangSuDungOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Chưa sử dụng", Text = "Chưa sử dụng" },
                    new SelectListItem { Value = "Đã check-in", Text = "Đã check-in" },
                    new SelectListItem { Value = "Đã check-out", Text = "Đã check-out" },
                    new SelectListItem { Value = "Chờ xử lý", Text = "Chờ xử lý" }
                };
                return View(model);
            }
        }

        // GET: Hiển thị form xác nhận xóa phiếu đặt phòng
        public async Task<IActionResult> Delete(int? id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id == null)
            {
                return NotFound();
            }

            var phieuDatPhong = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .Include(p => p.KhuyenMai)
                .FirstOrDefaultAsync(m => m.PhieuDatPhongId == id);

            if (phieuDatPhong == null)
            {
                return NotFound();
            }

            // Kiểm tra ràng buộc ChiTietPhieuPhong
            var hasChiTiet = await _context.ChiTietPhieuPhongs
                .AnyAsync(ct => ct.PhieuDatPhongId == id);

            // Kiểm tra ràng buộc HoaDonPdp
            var hasHoaDon = await _context.HoaDonPdps
                .AnyAsync(hd => hd.PhieuDatPhongId == id);

            ViewBag.HasChiTiet = hasChiTiet;
            ViewBag.HasHoaDon = hasHoaDon;
            return View(phieuDatPhong);
        }

        // POST: PhieuDatPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phieuDatPhong = await _context.PhieuDatPhongs
                .Include(p => p.ChiTietPhieuPhongs)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == id);

            if (phieuDatPhong == null)
            {
                return NotFound();
            }

            // Kiểm tra ràng buộc ChiTietPhieuPhong
            if (phieuDatPhong.ChiTietPhieuPhongs.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa phiếu đặt phòng vì còn chi tiết phiếu phòng liên quan.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra ràng buộc HoaDonPdp nếu có
            if (phieuDatPhong.HoaDonPdp != null)
            {
                TempData["ErrorMessage"] = "Không thể xóa phiếu đặt phòng vì đã có hóa đơn liên quan.";
                return RedirectToAction(nameof(Index));
            }

            _context.PhieuDatPhongs.Remove(phieuDatPhong);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa phiếu đặt phòng thành công.";
            return RedirectToAction(nameof(Index));
        }
        // GET: In phiếu đặt phòng
        [AuthorizePermission("ManagePhieuDatPhong")]
        public async Task<IActionResult> PrintPhieuDatPhong(int id)
        {
            var phieu = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .ThenInclude(k => k.TaiKhoan)
                .Include(p => p.ChiTietPhieuPhongs)
                .ThenInclude(c => c.Phong)
                .ThenInclude(p => p!.LoaiPhong)
                .Include(p => p.KhuyenMai)
                .FirstOrDefaultAsync(p => p.PhieuDatPhongId == id);

            if (phieu == null)
            {
                TempData["Error"] = "Phiếu đặt phòng không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(phieu);
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
                    tinhTrangSuDung = phieu.TinhTrangSuDung,
                    idvnpay = phieu.VnpTransactionId,
                    idmomo = phieu.MoMoTransactionId
                }
            });
        }

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