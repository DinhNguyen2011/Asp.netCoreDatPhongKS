namespace Asp.netCoreDatPhongKS.Models.ViewModels
{
    public class ThongtindatPhongViewModel
    {
        public int PhongId { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int SoDem => (Checkout - Checkin).Days;

        // Thông tin khách hàng
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Cccd { get; set; }
        public string GhiChu { get; set; }
        // Thông tin phòng
        public Phong? Phong { get; set; }
    }
}
