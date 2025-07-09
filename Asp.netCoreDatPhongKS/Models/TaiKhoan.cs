using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class TaiKhoan
    {
        public TaiKhoan()
        {
            DanhGia = new HashSet<DanhGium>();
            KhachHangs = new HashSet<KhachHang>();
            LienHeVoiCtois = new HashSet<LienHeVoiCtoi>();
            NhanViens = new HashSet<NhanVien>();
            QuyenTaiKhoans = new HashSet<QuyenTaiKhoan>();
        }

        public int TaiKhoanId { get; set; }

        [DisplayName("Địa chỉ Email")]
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        //[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string? MatKhau { get; set; }
        public int? VaiTroId { get; set; }
        public bool? TrangThai { get; set; }
        public string? HinhAnh { get; set; }
        public string? Hoten { get; set; }
        public DateTime? NgayTao { get; set; }
        [ValidateNever]
        public virtual VaiTro? VaiTro { get; set; }
        [ValidateNever]

        public virtual ICollection<DanhGium> DanhGia { get; set; }
        [ValidateNever]

        public virtual ICollection<KhachHang> KhachHangs { get; set; }
        [ValidateNever]

        public virtual ICollection<LienHeVoiCtoi> LienHeVoiCtois { get; set; }
        [ValidateNever]

        public virtual ICollection<NhanVien> NhanViens { get; set; }
        [ValidateNever]

        public virtual ICollection<QuyenTaiKhoan> QuyenTaiKhoans { get; set; }
    }
}
