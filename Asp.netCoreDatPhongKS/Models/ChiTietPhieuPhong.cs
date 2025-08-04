using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class ChiTietPhieuPhong
    {
        [Display(Name = "Chi tiết ID")]
        public int ChiTietId { get; set; }

        [Display(Name = "Phiếu đặt phòng ID")]
        public int? PhieuDatPhongId { get; set; }

        [Display(Name = "Phòng ID")]
        public int? PhongId { get; set; }

        [Display(Name = "Đơn giá")]
        public decimal? DonGia { get; set; }

        public virtual PhieuDatPhong? PhieuDatPhong { get; set; }
        public virtual Phong? Phong { get; set; }
    }
}
