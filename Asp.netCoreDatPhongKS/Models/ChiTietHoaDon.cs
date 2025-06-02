using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class ChiTietHoaDon
    {
        public int Id { get; set; }
        public int MaHoaDon { get; set; }
        public int? PhongId { get; set; }
        public int? DichVuId { get; set; }
        public string MoTa { get; set; } = null!;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        public virtual DichVu? DichVu { get; set; }
        public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
        public virtual Phong? Phong { get; set; }
    }
}
