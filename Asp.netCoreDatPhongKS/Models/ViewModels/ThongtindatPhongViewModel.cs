using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models.ViewModels
{
    public class ThongtindatPhongViewModel
    {
        public int PhongId { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int SoDem => (Checkout - Checkin).Days;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số CCCD")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải có đúng 12 chữ số")]
        [Display(Name = "Số CCCD")]
        public string Cccd { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
        // Thông tin phòng
        public Phong? Phong { get; set; }
    }
}
