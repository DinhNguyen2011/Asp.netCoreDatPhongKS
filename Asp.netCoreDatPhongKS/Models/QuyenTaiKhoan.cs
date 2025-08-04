using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class QuyenTaiKhoan
    {
        [Display(Name = "Tài khoản ID")]
        public int TaiKhoanId { get; set; }

        [Display(Name = "Quyền ID")]
        public int QuyenId { get; set; }

        [Display(Name = "Mô tả")]
        public string? Mota { get; set; }

        public virtual Quyen Quyen { get; set; } = null!;
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;
    }
}
