using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class LoaiPhong
    {
        public LoaiPhong()
        {
            Phongs = new HashSet<Phong>();
        }

        public int LoaiPhongId { get; set; }
        public string TenLoai { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal GiaCoBan { get; set; }
        public int SoluongNguoi { get; set; }
        public string? AnhDemo { get; set; }
        public DateTime? NgayTao { get; set; }

        public virtual ICollection<Phong> Phongs { get; set; }
    }
}
