using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class ChiTietPhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public ChiTietPhongController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ChiTiet(int phongId)
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            // Tìm phòng theo phongId
            var phong = await _context.Phongs
                .Include(p => p.LoaiPhong) // Bao gồm thông tin loại phòng
                .Include(p => p.DanhGia)   // Bao gồm danh sách đánh giá (nếu cần)
                .FirstOrDefaultAsync(p => p.PhongId == phongId);

            if (phong == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy phòng
            }

            return View(phong); // Truyền đối tượng phong sang view
        }
    }
}