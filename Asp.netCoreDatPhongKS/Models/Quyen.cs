using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class Quyen
    {
        public Quyen()
        {
            QuyenTaiKhoans = new HashSet<QuyenTaiKhoan>();
        }

        [Display(Name = "Quyền ID")]
        public int QuyenId { get; set; }

        [Display(Name = "Mã quyền")]
        public string MaQuyen { get; set; } = null!;

        [Display(Name = "Tên quyền")]
        public string TenQuyen { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        public virtual ICollection<QuyenTaiKhoan> QuyenTaiKhoans { get; set; }
    }
}
