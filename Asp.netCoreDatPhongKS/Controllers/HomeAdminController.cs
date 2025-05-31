using Asp.netCoreDatPhongKS.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCoreDatPhongKS.Controllers
{
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
            return View();
         
        }
    }
}