using Microsoft.AspNetCore.Mvc;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class HoiThaoSuKienController : Controller
    {
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            return View();
        }
    }
}
