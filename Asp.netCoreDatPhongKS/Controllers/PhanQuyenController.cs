using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [Route("phanquyen")]
    [AuthorizePermission("ManagePhanQuyen")]
    public class PhanQuyenController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public PhanQuyenController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách tài khoản Nhân viên/Lễ tân và quyền
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var taiKhoans = await _context.TaiKhoans
                .Include(t => t.VaiTro)
                .Include(t => t.QuyenTaiKhoans)
                .ThenInclude(tq => tq.Quyen)
                .Where(t => t.VaiTroId == 2) // Chỉ lấy Nhân viên/Lễ tân
                .ToListAsync();
            var quyen = await _context.Quyens.ToListAsync();
            ViewBag.Quyen = quyen;
            return View(taiKhoans);
        }

        // Gán quyền cho tài khoản
        [HttpGet]
        [Route("edit/{taiKhoanId}")]
        public async Task<IActionResult> Edit(int taiKhoanId)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.VaiTro)
                .Include(t => t.QuyenTaiKhoans)
                .ThenInclude(tq => tq.Quyen)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && t.VaiTroId == 2);
            if (taiKhoan == null)
                return NotFound();

            var quyen = await _context.Quyens.ToListAsync();
            ViewBag.Quyen = quyen;
            return View(taiKhoan);
        }

        [HttpPost]
        [Route("edit/{taiKhoanId}")]
        public async Task<IActionResult> Edit(int taiKhoanId, List<int> selectedQuyenIds)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.QuyenTaiKhoans)
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId && t.VaiTroId == 2);
            if (taiKhoan == null)
                return NotFound();

            // Xóa quyền cũ
            _context.QuyenTaiKhoans.RemoveRange(taiKhoan.QuyenTaiKhoans);

            // Thêm quyền mới
            foreach (var quyenId in selectedQuyenIds)
            {
                _context.QuyenTaiKhoans.Add(new QuyenTaiKhoan
                {
                    TaiKhoanId = taiKhoanId,
                    QuyenId = quyenId
                });
            }

            await _context.SaveChangesAsync();

            // Cập nhật session nếu tài khoản đang đăng nhập
            var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (sessionTaiKhoanId == taiKhoanId.ToString())
            {
                var quyen = await _context.QuyenTaiKhoans
                    .Where(tq => tq.TaiKhoanId == taiKhoanId)
                    .Select(tq => tq.Quyen.MaQuyen)
                    .ToListAsync();
                HttpContext.Session.SetString("Quyen", JsonSerializer.Serialize(quyen));
            }

            TempData["Success"] = "Cập nhật quyền thành công.";
            return RedirectToAction("Index");
        }

        // Quản lý quyền: Danh sách quyền
        [HttpGet]
        [Route("quyen/index")]
        public async Task<IActionResult> QuyenIndex()
        {
            var quyen = await _context.Quyens.ToListAsync();
            return View(quyen);
        }

        // Thêm quyền mới
        [HttpGet]
        [Route("quyen/create")]
        public IActionResult CreateQuyen()
        {
            return View();
        }

        [HttpPost]
        [Route("quyen/create")]
        public async Task<IActionResult> CreateQuyen(Quyen model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View(model);
            }

            var existingQuyen = await _context.Quyens.FirstOrDefaultAsync(q => q.MaQuyen == model.MaQuyen);
            if (existingQuyen != null)
            {
                TempData["Error"] = "Mã quyền đã tồn tại.";
                return View(model);
            }

            _context.Quyens.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm quyền thành công.";
            return RedirectToAction("QuyenIndex");
        }

        // Sửa quyền
        [HttpGet]
        [Route("quyen/edit/{quyenId}")]
        public async Task<IActionResult> EditQuyen(int quyenId)
        {
            var quyen = await _context.Quyens.FirstOrDefaultAsync(q => q.QuyenId == quyenId);
            if (quyen == null)
                return NotFound();

            return View(quyen);
        }

        [HttpPost]
        [Route("quyen/edit/{quyenId}")]
        public async Task<IActionResult> EditQuyen(int quyenId, Quyen model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View(model);
            }

            var quyen = await _context.Quyens.FirstOrDefaultAsync(q => q.QuyenId == quyenId);
            if (quyen == null)
                return NotFound();

            var existingQuyen = await _context.Quyens.FirstOrDefaultAsync(q => q.MaQuyen == model.MaQuyen && q.QuyenId != quyenId);
            if (existingQuyen != null)
            {
                TempData["Error"] = "Mã quyền đã tồn tại.";
                return View(model);
            }

            quyen.MaQuyen = model.MaQuyen;
            quyen.TenQuyen = model.TenQuyen;
            quyen.MoTa = model.MoTa;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Sửa quyền thành công.";
            return RedirectToAction("QuyenIndex");
        }

        // Xóa quyền
        [HttpGet]
        [Route("quyen/delete/{quyenId}")]
        public async Task<IActionResult> DeleteQuyen(int quyenId)
        {
            var quyen = await _context.Quyens.FirstOrDefaultAsync(q => q.QuyenId == quyenId);
            if (quyen == null)
                return NotFound();

            return View(quyen);
        }

        [HttpPost]
        [Route("quyen/delete/{quyenId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuyenConfirmed(int quyenId)
        {
            var quyen = await _context.Quyens
                .Include(q => q.QuyenTaiKhoans)
                .FirstOrDefaultAsync(q => q.QuyenId == quyenId);
            if (quyen == null)
                return NotFound();

            // Xóa các liên kết trong TaiKhoan_Quyen
            _context.QuyenTaiKhoans.RemoveRange(quyen.QuyenTaiKhoans);
            // Xóa quyền
            _context.Quyens.Remove(quyen);
            await _context.SaveChangesAsync();

            // Cập nhật session cho các tài khoản bị ảnh hưởng
            var affectedTaiKhoanIds = await _context.QuyenTaiKhoans
                .Where(tq => tq.QuyenId == quyenId)
                .Select(tq => tq.TaiKhoanId)
                .Distinct()
                .ToListAsync();

            var sessionTaiKhoanId = HttpContext.Session.GetString("TaiKhoanId");
            if (affectedTaiKhoanIds.Contains(int.Parse(sessionTaiKhoanId ?? "0")))
            {
                var quyenList = await _context.QuyenTaiKhoans
                    .Where(tq => tq.TaiKhoanId == int.Parse(sessionTaiKhoanId))
                    .Select(tq => tq.Quyen.MaQuyen)
                    .ToListAsync();
                HttpContext.Session.SetString("Quyen", JsonSerializer.Serialize(quyenList));
            }

            TempData["Success"] = "Xóa quyền thành công.";
            return RedirectToAction("QuyenIndex");
        }
    }
}