using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DanhGium
    {
        public int DanhGiaId { get; set; }
        public int? PhongId { get; set; }
        [Required(ErrorMessage = "Điểm là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        public int? Diem { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public int TaiKhoanId { get; set; }
        public int? DichVuId { get; set; }

        public virtual DichVu? DichVu { get; set; }
        public virtual Phong? Phong { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;
    }
}
