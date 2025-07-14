using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Required(ErrorMessage = "Vui lòng nhập số phòng")]
        [DisplayName("Số phòng")]
        public string SoPhong { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn loại phòng")]
        [DisplayName("Loại phòng")]
        public int LoaiPhongId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [DisplayName("Mô tả")]
        public string MoTa { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tình trạng")]
        [DisplayName("Tình trạng")]
        public string TinhTrang { get; set; } = null!;

        [DisplayName("Hình ảnh chính")]
        public string? HinhAnh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá phòng")]
        [DisplayName("Giá phòng / đêm")]
        public decimal? GiaPhong1Dem { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng khách")]
        [DisplayName("Số lượng khách")]
        public int? SoLuongKhach { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [DisplayName("Hình ảnh phụ 1")]
        public string? HinhAnh1 { get; set; }

        [DisplayName("Hình ảnh phụ 2")]
        public string? HinhAnh2 { get; set; }

        [DisplayName("Hình ảnh phụ 3")]
        public string? HinhAnh3 { get; set; }

        [ValidateNever]

        public virtual LoaiPhong LoaiPhong { get; set; } = null!;
        [ValidateNever]
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }
        [ValidateNever]
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
