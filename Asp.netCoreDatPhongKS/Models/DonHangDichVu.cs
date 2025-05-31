using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DonHangDichVu
    {
        public DonHangDichVu()
        {
            ChiTietDichVus = new HashSet<ChiTietDichVu>();
        }

        public int DonHangId { get; set; }
        public int? KhachHangId { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
        public int? HoaDonId { get; set; }

        public virtual HoaDon? HoaDon { get; set; }
        public virtual KhachHang? KhachHang { get; set; }
        public virtual ICollection<ChiTietDichVu> ChiTietDichVus { get; set; }
    }
}
