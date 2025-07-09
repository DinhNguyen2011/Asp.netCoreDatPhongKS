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

        [DisplayName("Mã phòng")]
        public int PhongId { get; set; }

        [DisplayName("Số phòng")]
        [Required(ErrorMessage = "Vui lòng nhập số phòng.")]
        public string? SoPhong { get; set; }

        [DisplayName("Loại phòng")]
        [Required(ErrorMessage = "Vui lòng chọn loại phòng.")]
        public int? LoaiPhongId { get; set; }

        [DisplayName("Mô tả")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả.")]
        public string? MoTa { get; set; }

        [DisplayName("Tình trạng")]
        [Required(ErrorMessage = "Vui lòng nhập tình trạng.")]
        public string? TinhTrang { get; set; }

        [DisplayName("Hình ảnh chính")]
        public string? HinhAnh { get; set; }

        [DisplayName("Giá phòng 1 đêm")]
        [Required(ErrorMessage = "Vui lòng nhập giá phòng 1 đêm.")]
        public decimal? GiaPhong1Dem { get; set; }

        [DisplayName("Số lượng khách tối đa")]
        [Required(ErrorMessage = "Vui lòng nhập số lượng khách tối đa.")]
        public int? SoLuongKhach { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [DisplayName("Hình ảnh phụ 1")]
        public string? HinhAnh1 { get; set; }

        [DisplayName("Hình ảnh phụ 2")]
        public string? HinhAnh2 { get; set; }

        [DisplayName("Hình ảnh phụ 3")]
        public string? HinhAnh3 { get; set; }

        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
