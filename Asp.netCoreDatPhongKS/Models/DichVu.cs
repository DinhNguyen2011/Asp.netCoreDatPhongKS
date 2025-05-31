using System;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class DichVu
    {
        public DichVu()
        {
            ChiTietDichVus = new HashSet<ChiTietDichVu>();
            DanhGia = new HashSet<DanhGium>();
        }

        public int DichVuId { get; set; }
        public string TenDichVu { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal DonGia { get; set; }
        public string? HinhAnh { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        public virtual ICollection<ChiTietDichVu> ChiTietDichVus { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
