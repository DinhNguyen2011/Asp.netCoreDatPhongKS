using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DanhGium
    {
        [Display(Name = "Đánh giá ID")]
        public int DanhGiaId { get; set; }

        [Display(Name = "Phòng ID")]
        public int? PhongId { get; set; }

        [Display(Name = "Dịch vụ ID")]
        public int? DichVuId { get; set; }

        [Display(Name = "Phiếu đặt phòng ID")]
        public int? PhieuDatPhongId { get; set; }

        [Display(Name = "Đơn hàng dịch vụ ID")]
        public int? DonHangDichVuId { get; set; }

        [Display(Name = "Điểm đánh giá")]
        public int? Diem { get; set; }

        [Display(Name = "Nội dung đánh giá")]
        public string? NoiDung { get; set; }

        [Display(Name = "Ngày đánh giá")]
        public DateTime? NgayDanhGia { get; set; }

        [Display(Name = "Tài khoản ID")]
        public int TaiKhoanId { get; set; }

        public virtual DichVu? DichVu { get; set; }
        public virtual DonHangDichVu? DonHangDichVu { get; set; }
        public virtual PhieuDatPhong? PhieuDatPhong { get; set; }
        public virtual Phong? Phong { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;
    }
}
