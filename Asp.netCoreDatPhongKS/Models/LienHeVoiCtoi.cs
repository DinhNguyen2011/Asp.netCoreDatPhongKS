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

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [DisplayName("Họ tên")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không đúng định dạng")]
        [DisplayName("Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại Việt Nam không hợp lệ")]
        [DisplayName("Số điện thoại")]
        public string SoDienThoai { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [DisplayName("Nội dung liên hệ")]
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
