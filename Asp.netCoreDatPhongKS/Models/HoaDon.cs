using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class HoaDon
    {
        public HoaDon()
        {
            DonHangDichVus = new HashSet<DonHangDichVu>();
            PhieuDatPhongs = new HashSet<PhieuDatPhong>();
        }

        public int HoaDonId { get; set; }
        public decimal TongTienPhong { get; set; }
        public decimal TongTienDichVu { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayLap { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? PhuongThucThanhToan { get; set; }
        public string? GhiChu { get; set; }
        public decimal? SoTienConThieu { get; set; }

        public virtual ICollection<DonHangDichVu> DonHangDichVus { get; set; }
        public virtual ICollection<PhieuDatPhong> PhieuDatPhongs { get; set; }
    }
}
