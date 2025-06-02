using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class LienHeVoiCtoi
    {
        public int LienHeId { get; set; }
        public int? KhachHangId { get; set; }
        public int? TaiKhoanId { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string NoiDung { get; set; } = null!;
        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }

        public virtual KhachHang? KhachHang { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
