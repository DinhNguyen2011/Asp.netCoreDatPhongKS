using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class ChiTietDichVu
    {
        public int ChiTietDichVuId { get; set; }
        public int PhieuDatPhongId { get; set; }
        public int DichVuId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public DateTime? NgayDatDichVu { get; set; }
        public string? TrangThaiSuDung { get; set; }
        public int? DonHangId { get; set; }

        public virtual DichVu DichVu { get; set; } = null!;
        public virtual DonHangDichVu? DonHang { get; set; }
        public virtual PhieuDatPhong PhieuDatPhong { get; set; } = null!;
    }
}
