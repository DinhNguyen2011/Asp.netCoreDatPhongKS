using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class ChiTietPhieuPhong
    {
        public int ChiTietId { get; set; }
        public int? PhieuDatPhongId { get; set; }
        public int? PhongId { get; set; }
        public decimal? DonGia { get; set; }

        public virtual PhieuDatPhong? PhieuDatPhong { get; set; }
        public virtual Phong? Phong { get; set; }
    }
}
