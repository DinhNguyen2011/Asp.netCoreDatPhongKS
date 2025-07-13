using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DanhGium
    {
        public int DanhGiaId { get; set; }
        public int? PhongId { get; set; }
        public int? DichVuId { get; set; }
        public int? PhieuDatPhongId { get; set; }
        public int? DonHangDichVuId { get; set; }
        public int? Diem { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public int TaiKhoanId { get; set; }

        public virtual DichVu? DichVu { get; set; }
        public virtual DonHangDichVu? DonHangDichVu { get; set; }
        public virtual PhieuDatPhong? PhieuDatPhong { get; set; }
        public virtual Phong? Phong { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;
    }
}
