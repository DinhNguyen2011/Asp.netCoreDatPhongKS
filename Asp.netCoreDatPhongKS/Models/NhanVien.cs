using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class NhanVien
    {
        public int NhanVienId { get; set; }
        public string HoTen { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? TaiKhoanId { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime? NgayTao { get; set; }

        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
