using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            DonHangDichVus = new HashSet<DonHangDichVu>();
            HoaDons = new HashSet<HoaDon>();
            LienHeVoiCtois = new HashSet<LienHeVoiCtoi>();
            PhieuDatPhongs = new HashSet<PhieuDatPhong>();
        }

        public int KhachHangId { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = null!;

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "CCCD không được để trống")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải gồm đúng 12 chữ số")]
        [Display(Name = "CCCD")]
        public string Cccd { get; set; } = null!;

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.Date)]
        public DateTime? NgayTao { get; set; }

        [Display(Name = "Tài khoản")]
        public int? TaiKhoanId { get; set; }

        [ValidateNever]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [ValidateNever]
        public virtual ICollection<DonHangDichVu> DonHangDichVus { get; set; }

        [ValidateNever]
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        [ValidateNever]
        public virtual ICollection<LienHeVoiCtoi> LienHeVoiCtois { get; set; }

        [ValidateNever]
        public virtual ICollection<PhieuDatPhong> PhieuDatPhongs { get; set; }
    }
}
