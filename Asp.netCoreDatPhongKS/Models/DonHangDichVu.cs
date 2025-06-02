using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DonHangDichVu
    {
        public DonHangDichVu()
        {
            ChiTietDonHangDichVus = new HashSet<ChiTietDonHangDichVu>();
            HoaDonDichVus = new HashSet<HoaDonDichVu>();
        }

        public int MaDonHangDv { get; set; }
        public int? KhachHangId { get; set; }
        public DateTime? NgayDat { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }

        public virtual KhachHang? KhachHang { get; set; }
        public virtual ICollection<ChiTietDonHangDichVu> ChiTietDonHangDichVus { get; set; }
        public virtual ICollection<HoaDonDichVu> HoaDonDichVus { get; set; }
    }
}
