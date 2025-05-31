using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [Route("datphong")]
    public class DatPhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public DatPhongController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [AuthorizePermission("BookRoom", "CreateBooking", "ManageDatPhong")]
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.PhieuDatPhongs
                .Include(p => p.KhachHang)
                .ToListAsync();
            return View(bookings);
        }

        [AuthorizePermission("BookRoom", "CreateBooking")]
        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [AuthorizePermission("CreateBooking")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PhieuDatPhong model)
        {
            if (ModelState.IsValid)
            {
                _context.PhieuDatPhongs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phiếu đặt phòng thành công.";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}