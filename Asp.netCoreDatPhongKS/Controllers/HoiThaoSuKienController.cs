using Asp.netCoreDatPhongKS.Models;
using Asp.netCoreDatPhongKS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class HoiThaoSuKienController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public HoiThaoSuKienController(HotelPlaceVipContext context)
        {
            _context = context;
           
        }
        private IActionResult RestrictAccessByVaiTro()
        {
            string userName = HttpContext.Session.GetString("Hoten");

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
    }
}
