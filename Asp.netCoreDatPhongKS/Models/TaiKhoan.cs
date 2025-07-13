using System;
using System.Collections.Generic;

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
        public string Email { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public int? VaiTroId { get; set; }
        public bool? TrangThai { get; set; }
        public string? HinhAnh { get; set; }
        public string? Hoten { get; set; }
        public DateTime? NgayTao { get; set; }

        public virtual VaiTro? VaiTro { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
        public virtual ICollection<LienHeVoiCtoi> LienHeVoiCtois { get; set; }
        public virtual ICollection<NhanVien> NhanViens { get; set; }
        public virtual ICollection<QuyenTaiKhoan> QuyenTaiKhoans { get; set; }
    }
}
