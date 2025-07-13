using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class ChiTietDonHangDichVu
    {
        public int Id { get; set; }

        [DisplayName("Mã đơn hàng dịch vụ")]
        public int MaDonHangDv { get; set; }

        [DisplayName("Dịch vụ")]
        public int DichVuId { get; set; }

        [DisplayName("Số lượng")]
        public int SoLuong { get; set; }

        [DisplayName("Đơn giá")]
        public decimal DonGia { get; set; }

        [DisplayName("Thành tiền")]
        public decimal? ThanhTien { get; set; }

        public virtual DichVu DichVu { get; set; } = null!;


        public virtual DonHangDichVu MaDonHangDvNavigation { get; set; } = null!;
    }
}
