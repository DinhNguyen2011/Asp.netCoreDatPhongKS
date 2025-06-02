using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            DonHangDichVus = new HashSet<DonHangDichVu>();
            HoaDons = new HashSet<HoaDon>();
            LienHeVoiCtois = new HashSet<LienHeVoiCtoi>();
            PhieuDatPhongs = new HashSet<PhieuDatPhong>();
        }

        public int KhachHangId { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? Cccd { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? TaiKhoanId { get; set; }

        public virtual TaiKhoan? TaiKhoan { get; set; }
        public virtual ICollection<DonHangDichVu> DonHangDichVus { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<LienHeVoiCtoi> LienHeVoiCtois { get; set; }
        public virtual ICollection<PhieuDatPhong> PhieuDatPhongs { get; set; }
    }
}
