using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class VaiTro
    {
        public VaiTro()
        {
            TaiKhoans = new HashSet<TaiKhoan>();
        }

        public int VaiTroId { get; set; }
        public string? TenVaiTro { get; set; }

        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
    }
}
