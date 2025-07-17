using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class NhanVien
    {
        public int NhanVienId { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; } = null!;

        [Display(Name = "CCCD")]
        [Required(ErrorMessage = "Vui lòng nhập CCCD")]
        public string Cccd { get; set; } = null!;

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string DiaChi { get; set; } = null!;


        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = null!;


        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [RegularExpression(@"^[\w\.\-]+@([\w\-]+\.)+[a-zA-Z]{2,4}$", ErrorMessage = "Email không hợp lệ.")]
        [DisplayName("Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Tài khoản")]
        public int? TaiKhoanId { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
