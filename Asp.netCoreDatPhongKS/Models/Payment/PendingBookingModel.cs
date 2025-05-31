namespace Asp.netCoreDatPhongKS.Models.Payment
{
    //bảng tam này tạo ra để lưu thông tin bảng tạm nếu thanh toán thất bại thì sẽ database trống
    public class PendingBookingModel
    {
        public int PhongId { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int SoDem { get; set; }
        public decimal TongTien { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Cccd { get; set; }
        public string GhiChu { get; set; }
    }
}
