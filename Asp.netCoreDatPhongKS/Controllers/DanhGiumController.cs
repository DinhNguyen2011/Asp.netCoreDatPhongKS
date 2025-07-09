using Asp.netCoreDatPhongKS.Filters;
using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Controllers
{
    [RestrictToAdmin]

    public class DanhGiumController : Controller
    {
        private readonly HotelPlaceVipContext _context;

        public DanhGiumController(HotelPlaceVipContext context)
        {
            _context = context;
        }

        // GET: DanhGium/Index
    }
}