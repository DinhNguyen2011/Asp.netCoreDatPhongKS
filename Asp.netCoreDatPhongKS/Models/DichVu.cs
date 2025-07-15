using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DichVu
    {
        public DichVu()
        {
            ChiTietDonHangDichVus = new HashSet<ChiTietDonHangDichVu>();
            DanhGia = new HashSet<DanhGium>();
        }

        public int DichVuId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [DisplayName("Tên dịch vụ")]
        public string TenDichVu { get; set; } = null!;

        [DisplayName("Mô tả")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        [Range(1, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [DisplayName("Đơn giá")]

        public decimal DonGia { get; set; }

        [DisplayName("Hình ảnh")]
        public string? HinhAnh { get; set; }

        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [DisplayName("Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }

        public virtual ICollection<ChiTietDonHangDichVu> ChiTietDonHangDichVus { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
