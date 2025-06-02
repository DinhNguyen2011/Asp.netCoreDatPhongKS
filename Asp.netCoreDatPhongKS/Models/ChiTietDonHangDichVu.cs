using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class ChiTietDonHangDichVu
    {
        public int Id { get; set; }
        public int MaDonHangDv { get; set; }
        public int DichVuId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal? ThanhTien { get; set; }

        public virtual DichVu DichVu { get; set; } = null!;
        public virtual DonHangDichVu MaDonHangDvNavigation { get; set; } = null!;
    }
}
