using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("khachhang")]
public class KhachHangController : Controller
{
    private readonly HotelPlaceVipContext _context;

    public KhachHangController(HotelPlaceVipContext context)
    {
        _context = context;
    }

    [AuthorizePermission("ViewCus", "ManageKhachHang")]
    [HttpGet]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var customers = await _context.KhachHangs.ToListAsync();
        return View(customers);
    }

    [AuthorizePermission("CreateCus", "ManageKhachHang")]
    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [AuthorizePermission("CreateCus", "ManageKhachHang")]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(KhachHang model)
    {
        if (ModelState.IsValid)
        {
            _context.KhachHangs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm khách hàng thành công.";
            return RedirectToAction("Index");
        }
        return View(model);
    }
}