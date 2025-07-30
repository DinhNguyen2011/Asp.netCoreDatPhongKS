using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class PhieuDatPhong
    {
        public PhieuDatPhong()
        {
            ChiTietPhieuPhongs = new HashSet<ChiTietPhieuPhong>();
            DanhGia = new HashSet<DanhGium>();
        }

        public int PhieuDatPhongId { get; set; }

        [Display(Name = "Mã phiếu")]
        public string? MaPhieu { get; set; }

        [Display(Name = "Khách hàng")]
        public int? KhachHangId { get; set; }

        [Display(Name = "Ngày đặt")]
       // [DataType(DataType.Date)]
        public DateTime? NgayDat { get; set; }

      //  [Required(ErrorMessage = "Ngày nhận không được để trống")]
        [Display(Name = "Ngày nhận")]
      //  [DataType(DataType.Date)]
        public DateTime? NgayNhan { get; set; }

      //  [Required(ErrorMessage = "Ngày trả không được để trống")]
        [Display(Name = "Ngày trả")]
       // [DataType(DataType.DateTime)]
        public DateTime? NgayTra { get; set; }

        [Display(Name = "Khuyến mãi")]
        public int? KhuyenMaiId { get; set; }

        //[Required(ErrorMessage = "Tổng tiền không được để trống")]
        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)] //Gợi ý định dạng hiển thị tiền tệ 
        public decimal TongTien { get; set; }

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        [Display(Name = "Mã giao dịch VNPay")]
        public string? VnpTransactionId { get; set; }

        [Display(Name = "Số tiền cọc")]
        [DataType(DataType.Currency)]
        public decimal? SoTienCoc { get; set; }

        [Display(Name = "Tình trạng sử dụng")]
        public string? TinhTrangSuDung { get; set; }

        [Display(Name = "Số tiền đã thanh toán")]
       // [DataType(DataType.Currency)]
        public decimal? SoTienDaThanhToan { get; set; }

        [Display(Name = "Mã giao dịch MoMo")]
        public string? MoMoTransactionId { get; set; }

        [ValidateNever]
        public virtual KhachHang? KhachHang { get; set; }

        [ValidateNever]
        public virtual KhuyenMai? KhuyenMai { get; set; }

        [ValidateNever]
        public virtual HoaDonPdp? HoaDonPdp { get; set; }

        [ValidateNever]
        public virtual ICollection<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; }

        [ValidateNever]
        public virtual ICollection<DanhGium> DanhGia { get; set; }
    }
}
