using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class LienHeVoiCtoi
    {
        public int LienHeId { get; set; }

        [DisplayName("Khách hàng")]
        public int? KhachHangId { get; set; }

        [DisplayName("Tài khoản")]
        public int? TaiKhoanId { get; set; }

        [DisplayName("Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; } = null!;

        [DisplayName("Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [DisplayName("Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [DisplayName("Nội dung")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ")]
        public string NoiDung { get; set; } = null!;

        [DisplayName("Ngày gửi")]
        public DateTime NgayGui { get; set; }

        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }
        [DisplayName("Trạng thái")]
        public string? TrangThai { get; set; }

        public virtual KhachHang? KhachHang { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
