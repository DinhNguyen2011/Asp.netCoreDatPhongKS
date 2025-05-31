using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Số phòng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Số phòng không được vượt quá 50 ký tự")]
        public string? SoPhong { get; set; }
        [Required(ErrorMessage = "Loại phòng là bắt buộc")]
        public int? LoaiPhongId { get; set; }
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public string? TinhTrang { get; set; }
        public string? HinhAnh { get; set; }
        public decimal? GiaPhong1Dem { get; set; }
        public int? SoLuongKhach { get; set; }
        public DateTime? NgayTao { get; set; }
        public string? HinhAnh1 { get; set; }
        public string? HinhAnh2 { get; set; }
        public string? HinhAnh3 { get; set; }

        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
