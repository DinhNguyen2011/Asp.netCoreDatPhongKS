using Asp.netCoreDatPhongKS.Models;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCoreDatPhongKS.Controllers
{
    public class LoaiPhongController : Controller
    {
        private HotelPlaceVipContext db = new HotelPlaceVipContext();
        public IActionResult Index(string loai, int? minGia, int? maxGia, int? soNguoi)
        {
            var danhSach = db.LoaiPhongs.AsQueryable();

            if (!string.IsNullOrEmpty(loai))
                danhSach = danhSach.Where(lp => lp.TenLoai.Contains(loai));

            if (minGia.HasValue)
                danhSach = danhSach.Where(lp => lp.GiaCoBan >= minGia);

            if (maxGia.HasValue)
                danhSach = danhSach.Where(lp => lp.GiaCoBan <= maxGia);

            if (soNguoi.HasValue)
                danhSach = danhSach.Where(lp => lp.SoluongNguoi == soNguoi);

            ViewBag.Loai = loai;
            ViewBag.MinGia = minGia;
            ViewBag.MaxGia = maxGia;
            ViewBag.SoNguoi = soNguoi;

            return View(danhSach.ToList());
        }


    }
}
