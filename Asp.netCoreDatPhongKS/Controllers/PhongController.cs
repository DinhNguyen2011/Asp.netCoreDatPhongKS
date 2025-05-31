using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [Route("phong")]
    public class PhongController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public PhongController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        [AuthorizePermission("ViewRoom", "ManageRoom")]
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .ToListAsync();
            return View(rooms);
        }

        [AuthorizePermission("ManageRoom")]
        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [AuthorizePermission("ManageRoom")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Phong model)
        {
            if (ModelState.IsValid)
            {
                _context.Phongs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phòng thành công.";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}