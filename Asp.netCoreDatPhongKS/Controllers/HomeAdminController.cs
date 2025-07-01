using Asp.netCoreDatPhongKS.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("Hoten");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Hoten"] = userName;
            }
            string email = HttpContext.Session.GetString("Email");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewData["Email"] = email;
            }
            return View();
         
        }
    }
}