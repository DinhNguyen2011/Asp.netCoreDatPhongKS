using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class Phong
    {
        public Phong()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
            ChiTietPhieuPhongs = new HashSet<ChiTietPhieuPhong>();
            DanhGia = new HashSet<DanhGium>();
        }

        public int PhongId { get; set; }
        public string? SoPhong { get; set; }
        public int? LoaiPhongId { get; set; }
        public string? MoTa { get; set; }
        public string? TinhTrang { get; set; }
        public string? HinhAnh { get; set; }
        public decimal? GiaPhong1Dem { get; set; }
        public int? SoLuongKhach { get; set; }
        public DateTime? NgayTao { get; set; }
        public string? HinhAnh1 { get; set; }
        public string? HinhAnh2 { get; set; }
        public string? HinhAnh3 { get; set; }

        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
