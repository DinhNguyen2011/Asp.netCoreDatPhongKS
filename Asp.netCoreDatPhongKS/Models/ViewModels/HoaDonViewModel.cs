namespace Asp.netCoreDatPhongKS.Models.ViewModels
{
    public class HoaDonViewModel
    {
        public int Id { get; set; } // MaHoaDon hoặc Id của HoaDonDichVu
        public string Loai { get; set; } // "Hóa đơn tổng" hoặc "Dịch vụ vãng lai"
        public DateTime? NgayLap { get; set; }
        public string KhachHangTen { get; set; }
        public string KhachHangCCCD { get; set; }
        public string NhanVienTen { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public HoaDon HoaDon { get; set; } // Cho hóa đơn tổng
        public HoaDonDichVu HoaDonDichVu { get; set; } // Cho hóa đơn dịch vụ vãng lai
    }
}
