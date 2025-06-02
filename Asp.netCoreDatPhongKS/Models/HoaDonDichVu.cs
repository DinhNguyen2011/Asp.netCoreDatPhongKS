using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class HoaDonDichVu
    {
        public int Id { get; set; }
        public int MaDonHangDv { get; set; }
        public string TrangThaiThanhToan { get; set; } = null!;
        public int? MaHoaDonTong { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string? HinhThucThanhToan { get; set; }

        public virtual DonHangDichVu MaDonHangDvNavigation { get; set; } = null!;
        public virtual HoaDon? MaHoaDonTongNavigation { get; set; }
    }
}
