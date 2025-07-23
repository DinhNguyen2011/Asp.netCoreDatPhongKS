using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]
    public class KhachHangController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public KhachHangController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: KhachHang/Index
        public IActionResult Index(string searchString)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var khachHangs = _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                khachHangs = khachHangs.Where(kh => (kh.HoTen != null && kh.HoTen.Contains(searchString)) ||
                                                    (kh.Email != null && kh.Email.Contains(searchString)) ||
                                                    (kh.SoDienThoai != null && kh.SoDienThoai.Contains(searchString)));
            }

            ViewBag.SearchString = searchString;

            return View(khachHangs.ToList());
        }

        // GET: KhachHang/Create
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        // POST: KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (ModelState.IsValid)
            {
         
                if (_context.TaiKhoans.Any(tk => tk.Email == khachHang.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(khachHang);
                }
                if (_context.KhachHangs.Any(kh => kh.Cccd == khachHang.Cccd))
                {
                    ModelState.AddModelError("Cccd", "Cccd đã có sẵn.");
                    return View(khachHang);
                }
                // Create TaiKhoan
                var taiKhoan = new TaiKhoan
                {
                    Email = khachHang.Email,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword ("123"), // Replace with secure password hashing
                    VaiTroId = 3, // Customer role
                    TrangThai = true,
                    Hoten = khachHang.HoTen,
                    NgayTao = DateTime.Now
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Link KhachHang to TaiKhoan
                khachHang.TaiKhoanId = taiKhoan.TaiKhoanId;
                khachHang.NgayTao = DateTime.Now;

                _context.Add(khachHang);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        // GET: KhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var khachHang = await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .FirstOrDefaultAsync(kh => kh.KhachHangId == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: KhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachHang khachHang)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id != khachHang.KhachHangId)
            {
                return NotFound();
            }

            var existingKhachHang = await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .AsNoTracking()
                .FirstOrDefaultAsync(kh => kh.KhachHangId == id);
            if (existingKhachHang == null)
            {
                return NotFound();
            }

            // Check if new Email is unique (excluding current TaiKhoan)
            if (_context.TaiKhoans.Any(tk => tk.Email == khachHang.Email && tk.TaiKhoanId != khachHang.TaiKhoanId))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(khachHang);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve NgayTao
                    khachHang.NgayTao = existingKhachHang.NgayTao;

                    // Update TaiKhoan Email and Hoten if changed
                    if (khachHang.TaiKhoanId.HasValue)
                    {
                        var taiKhoan = await _context.TaiKhoans.FindAsync(khachHang.TaiKhoanId);
                        if (taiKhoan != null)
                        {
                            taiKhoan.Email = khachHang.Email;
                            taiKhoan.Hoten = khachHang.HoTen;
                            _context.Update(taiKhoan);
                        }
                    }

                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.KhachHangs.Any(e => e.KhachHangId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                TempData["Success"] = "Sửa thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        // GET: KhachHang/Delete/5
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

            var khachHang = await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .Include(kh => kh.DonHangDichVus)
                .Include(kh => kh.HoaDons)
                .Include(kh => kh.LienHeVoiCtois)
                .Include(kh => kh.PhieuDatPhongs)
                .FirstOrDefaultAsync(kh => kh.KhachHangId == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            // Check foreign key constraints
            if (khachHang.DonHangDichVus.Any() || khachHang.HoaDons.Any() ||
                khachHang.LienHeVoiCtois.Any() || khachHang.PhieuDatPhongs.Any())
            {
                ViewBag.ErrorMessage = "Không thể xóa khách hàng này vì có dữ liệu liên quan (đơn hàng, hóa đơn, liên hệ, hoặc phiếu đặt phòng).";
            }

            return View(khachHang);
        }

        // POST: KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .Include(kh => kh.DonHangDichVus)
                .Include(kh => kh.HoaDons)
                .Include(kh => kh.LienHeVoiCtois)
                .Include(kh => kh.PhieuDatPhongs)
                .FirstOrDefaultAsync(kh => kh.KhachHangId == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            // Check foreign key constraints
            if (khachHang.DonHangDichVus.Any() || khachHang.HoaDons.Any() ||
                khachHang.LienHeVoiCtois.Any() || khachHang.PhieuDatPhongs.Any())
            {
                ViewBag.ErrorMessage = "Không thể xóa khách hàng này vì có dữ liệu liên quan (đơn hàng, hóa đơn, liên hệ, hoặc phiếu đặt phòng).";
                return View(khachHang);
            }

            // Delete TaiKhoan if it exists and has no other references
            if (khachHang.TaiKhoan != null)
            {
                var taiKhoan = await _context.TaiKhoans
                    .Include(tk => tk.KhachHangs)
                    .Include(tk => tk.DanhGia)
                    .Include(tk => tk.LienHeVoiCtois)
                    .Include(tk => tk.NhanViens)
                    .Include(tk => tk.QuyenTaiKhoans)
                    .FirstOrDefaultAsync(tk => tk.TaiKhoanId == khachHang.TaiKhoanId);
                if (taiKhoan != null &&
                    !taiKhoan.KhachHangs.Any(kh => kh.KhachHangId != khachHang.KhachHangId) &&
                    !taiKhoan.DanhGia.Any() &&
                    !taiKhoan.LienHeVoiCtois.Any() &&
                    !taiKhoan.NhanViens.Any() &&
                    !taiKhoan.QuyenTaiKhoans.Any())
                {
                    _context.TaiKhoans.Remove(taiKhoan);
                }
            }

            _context.KhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}