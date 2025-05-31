using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class PhieuDatPhong
    {
        public PhieuDatPhong()
        {
            ChiTietDichVus = new HashSet<ChiTietDichVu>();
            ChiTietPhieuPhongs = new HashSet<ChiTietPhieuPhong>();
        }

        public int PhieuDatPhongId { get; set; }
        public string? MaPhieu { get; set; }
        public int? KhachHangId { get; set; }
        public DateTime? NgayDat { get; set; }
        public DateTime? NgayNhan { get; set; }
        public DateTime? NgayTra { get; set; }
        public int? KhuyenMaiId { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThai { get; set; }
        public string? VnpTransactionId { get; set; }
        public decimal? SoTienCoc { get; set; }
        public string? TinhTrangSuDung { get; set; }
        public decimal? SoTienDaThanhToan { get; set; }
        public int? HoaDonId { get; set; }

        public virtual HoaDon? HoaDon { get; set; }
        public virtual KhachHang? KhachHang { get; set; }
        public virtual KhuyenMai? KhuyenMai { get; set; }
        public virtual ICollection<ChiTietDichVu> ChiTietDichVus { get; set; }
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }
    }
}
