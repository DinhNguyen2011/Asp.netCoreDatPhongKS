using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class QuyenTaiKhoan
    {
        public int TaiKhoanId { get; set; }
        public int QuyenId { get; set; }
        public string? Mota { get; set; }

        public virtual Quyen Quyen { get; set; } = null!;
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;
    }
}
