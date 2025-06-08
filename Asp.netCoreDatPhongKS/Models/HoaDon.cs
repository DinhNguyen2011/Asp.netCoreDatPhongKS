using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class HoaDon
    {
        public HoaDon()
        {
            HoaDonDichVus = new HashSet<HoaDonDichVu>();
            HoaDonPdps = new HashSet<HoaDonPdp>();
        }

        public int MaHoaDon { get; set; }
        public DateTime? NgayLap { get; set; }
        public int? KhachHangId { get; set; }
        public decimal? TongTienPhong { get; set; }
        public decimal? TongTienDichVu { get; set; }
        public decimal? TongTien { get; set; }
        public string? HinhThucThanhToan { get; set; }
        public string? TrangThai { get; set; }
        public bool? IsKhachVangLai { get; set; }
        public string? GhiChu { get; set; }
        public decimal? SoTienConNo { get; set; }
        public string? NguoiLapDh { get; set; }

        public virtual KhachHang? KhachHang { get; set; }
        public virtual ICollection<HoaDonDichVu> HoaDonDichVus { get; set; }
        public virtual ICollection<HoaDonPdp> HoaDonPdps { get; set; }
    }
}
