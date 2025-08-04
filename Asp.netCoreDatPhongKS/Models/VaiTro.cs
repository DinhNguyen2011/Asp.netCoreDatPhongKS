using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class VaiTro
    {
        public VaiTro()
        {
            TaiKhoans = new HashSet<TaiKhoan>();
        }
        [Display(Name = "Vai trò ID")]
        public int VaiTroId { get; set; }
        [Display(Name = "Tên vai trò")]

        public string? TenVaiTro { get; set; }

        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
    }
}
