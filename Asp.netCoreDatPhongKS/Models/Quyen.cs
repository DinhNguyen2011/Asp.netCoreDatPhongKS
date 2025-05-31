using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class Quyen
    {
        public Quyen()
        {
            QuyenTaiKhoans = new HashSet<QuyenTaiKhoan>();
        }

        public int QuyenId { get; set; }
        public string MaQuyen { get; set; } = null!;
        public string TenQuyen { get; set; } = null!;
        public string? MoTa { get; set; }

        public virtual ICollection<QuyenTaiKhoan> QuyenTaiKhoans { get; set; }
    }
}
