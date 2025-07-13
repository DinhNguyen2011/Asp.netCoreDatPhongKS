using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class KhuyenMai
    {
        public KhuyenMai()
        {
            PhieuDatPhongs = new HashSet<PhieuDatPhong>();
        }

        public int KhuyenMaiId { get; set; }
        public string MaKhuyenMai { get; set; } = null!;
        public string? MoTa { get; set; }
        public int PhanTramGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? TrangThai { get; set; }

        public virtual ICollection<PhieuDatPhong> PhieuDatPhongs { get; set; }
    }
}
