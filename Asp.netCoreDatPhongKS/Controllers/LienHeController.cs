using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class LienHeController : Controller
    {
        private readonly HotelPlaceVipContext _context;
        private readonly IEmailService _emailService;

        public LienHeController(HotelPlaceVipContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Admin: List all contacts
        public async Task<IActionResult> Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var lienHeList = await _context.LienHeVoiCtois.ToListAsync();
            return View(lienHeList);
        }

        // Admin: View contact details
        public async Task<IActionResult> Details(int? id)
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

            var lienHe = await _context.LienHeVoiCtois
                .Include(l => l.KhachHang)
                .Include(l => l.TaiKhoan)
                .FirstOrDefaultAsync(m => m.LienHeId == id);

            if (lienHe == null)
            {
                return NotFound();
            }

            return View(lienHe);
        }

        // Admin: Create new contact (GET)
        public IActionResult Create()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }

        // Admin: Create new contact (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LienHeId,KhachHangId,TaiKhoanId,HoTen,Email,SoDienThoai,NoiDung,NgayGui,TrangThai,GhiChu")] LienHeVoiCtoi lienHe)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (ModelState.IsValid)
            {
                lienHe.NgayGui = DateTime.Now;
                _context.Add(lienHe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lienHe);
        }


        // Admin: Edit contact (GET)
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

            var lienHe = await _context.LienHeVoiCtois.FindAsync(id);
            if (lienHe == null)
            {
                return NotFound();
            }
            return View(lienHe);
        }

        // Admin: Edit contact (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LienHeId,KhachHangId,TaiKhoanId,HoTen,Email,SoDienThoai,NoiDung,TrangThai,GhiChu")] LienHeVoiCtoi lienHe)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            if (id != lienHe.LienHeId)
            {
                return NotFound();
            }

            // Lấy bản ghi gốc từ database để giữ NgayGui
            var existingLienHe = await _context.LienHeVoiCtois.AsNoTracking().FirstOrDefaultAsync(l => l.LienHeId == id);
            if (existingLienHe == null)
            {
                return NotFound();
            }

            // Giữ nguyên NgayGui từ bản ghi gốc
            lienHe.NgayGui = existingLienHe.NgayGui;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lienHe);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LienHeVoiCtoiExists(lienHe.LienHeId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lưu liên hệ: {ex.Message}";
                    return View(lienHe);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào: " + string.Join("; ", errors);
            }
            return View(lienHe);
        }

        // Admin: Delete contact (GET)
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

            var lienHe = await _context.LienHeVoiCtois
                .Include(l => l.KhachHang)
                .Include(l => l.TaiKhoan)
                .FirstOrDefaultAsync(m => m.LienHeId == id);

            if (lienHe == null)
            {
                return NotFound();
            }

            return View(lienHe);
        }

        // Admin: Delete contact (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            var lienHe = await _context.LienHeVoiCtois.FindAsync(id);
            if (lienHe != null)
            {
                _context.LienHeVoiCtois.Remove(lienHe);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult KhachHangLienHe()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View(new LienHeVoiCtoi { TrangThai = "Chưa xử lý" });
        }

        // Customer: Contact form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhachHangLienHe([Bind("HoTen,Email,SoDienThoai,NoiDung")] LienHeVoiCtoi lienHe)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            lienHe.TrangThai = "Chưa xử lý"; // Gán trước để tránh lỗi validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập vào: " + string.Join("; ", errors);
                return View(lienHe);
            }

            try
            {
                lienHe.NgayGui = DateTime.Now;
                _context.Add(lienHe);
                await _context.SaveChangesAsync();

                // Send confirmation email
                var emailBody = $@"<p>Cảm ơn bạn đã liên hệ với Khách sạn Thiềm Định.</p>
                               <p>Thông tin liên hệ của bạn:</p>
                               <ul>
                                   <li><strong>Họ tên:</strong> {lienHe.HoTen}</li>
                                   <li><strong>Email:</strong> {lienHe.Email}</li>
                                   <li><strong>Số điện thoại:</strong> {lienHe.SoDienThoai ?? "Không cung cấp"}</li>
                                   <li><strong>Nội dung:</strong> {lienHe.NoiDung}</li>
                               </ul>
                               <p>Vui lòng liên hệ chúng tôi nếu có bất kỳ câu hỏi nào. Hotline: 0853461030</p>";

                await _emailService.SendEmailAsync(lienHe.Email, "Xác nhận liên hệ - Khách sạn Thiềm Định", emailBody);

                TempData["SuccessMessage"] = "Liên hệ của bạn đã được gửi thành công! Vui lòng kiểm tra email để xem xác nhận.";
                return RedirectToAction(nameof(KhachHangLienHe));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi liên hệ. Vui lòng thử lại sau.";
                Console.WriteLine($"Error: {ex.Message}");
                return View(lienHe);
            }
        }
        private bool LienHeVoiCtoiExists(int id)
        {
            return _context.LienHeVoiCtois.Any(e => e.LienHeId == id);
        }
    }
}