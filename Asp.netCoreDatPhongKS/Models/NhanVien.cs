using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        public int NhanVienId { get; set; }
        public string? HoTen { get; set; }
        public string? Cccd { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public int? TaiKhoanId { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime? NgayTao { get; set; }

        public virtual TaiKhoan? TaiKhoan { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
