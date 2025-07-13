using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class Phong
    {
        public Phong()
        {
            ChiTietPhieuPhongs = new HashSet<ChiTietPhieuPhong>();
            DanhGia = new HashSet<DanhGium>();
        }

        public int PhongId { get; set; }
        public string SoPhong { get; set; } = null!;
        public int LoaiPhongId { get; set; }
        public string MoTa { get; set; } = null!;
        public string TinhTrang { get; set; } = null!;
        public string? HinhAnh { get; set; }
        public decimal? GiaPhong1Dem { get; set; }
        public int? SoLuongKhach { get; set; }
        public DateTime? NgayTao { get; set; }
        public string? HinhAnh1 { get; set; }
        public string? HinhAnh2 { get; set; }
        public string? HinhAnh3 { get; set; }
        [ValidateNever]

        public virtual LoaiPhong LoaiPhong { get; set; } = null!;
        [ValidateNever]
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }
        [ValidateNever]
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
