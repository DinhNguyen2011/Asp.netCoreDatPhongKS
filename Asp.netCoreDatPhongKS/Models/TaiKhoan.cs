using System;
using System.Collections.Generic;
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

        [Display(Name = "Tài khoản ID")]
        public int TaiKhoanId { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = null!;

        [Display(Name = "Vai trò")]
        public int? VaiTroId { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? TrangThai { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Họ tên")]
        public string? Hoten { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        public virtual VaiTro? VaiTro { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
        public virtual ICollection<LienHeVoiCtoi> LienHeVoiCtois { get; set; }
        public virtual ICollection<NhanVien> NhanViens { get; set; }
        public virtual ICollection<QuyenTaiKhoan> QuyenTaiKhoans { get; set; }
    }
}
