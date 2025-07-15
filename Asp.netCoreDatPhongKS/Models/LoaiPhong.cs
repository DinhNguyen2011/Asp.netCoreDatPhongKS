using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class LoaiPhong
    {
        public LoaiPhong()
        {
            Phongs = new HashSet<Phong>();
        }

        public int LoaiPhongId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại phòng")]
        [DisplayName("Tên loại phòng")]
        public string TenLoai { get; set; } = null!;

        [DisplayName("Mô tả")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá cơ bản")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn 0")]
        [DisplayName("Giá cơ bản")]
        public decimal GiaCoBan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng người")]
        [Range(1, double.MaxValue, ErrorMessage = "Số lượng người phải lớn hơn 0")]
        [DisplayName("Số lượng người")]
        public int SoluongNguoi { get; set; }

        [DisplayName("Ảnh demo")]
        public string? AnhDemo { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        public virtual ICollection<Phong> Phongs { get; set; }
    }
}
