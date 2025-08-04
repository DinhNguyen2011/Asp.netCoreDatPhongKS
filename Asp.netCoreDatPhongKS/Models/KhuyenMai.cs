using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class KhuyenMai
    {
        public KhuyenMai()
        {
            PhieuDatPhongs = new HashSet<PhieuDatPhong>();
        }

        [Display(Name = "Mã khuyến mãi ID")]
        public int KhuyenMaiId { get; set; }

        [Display(Name = "Mã khuyến mãi")]
        public string MaKhuyenMai { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Phần trăm giảm")]
        public int PhanTramGiam { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? TrangThai { get; set; }

        public virtual ICollection<PhieuDatPhong> PhieuDatPhongs { get; set; }
    }
}
