using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class HoaDonPdp
    {
        public int Id { get; set; }
        public int MaHoaDon { get; set; }
        public int PhieuDatPhongId { get; set; }

        public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
        public virtual PhieuDatPhong PhieuDatPhong { get; set; } = null!;
    }
}
