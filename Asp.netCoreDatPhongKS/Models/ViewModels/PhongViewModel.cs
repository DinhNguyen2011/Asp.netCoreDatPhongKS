namespace Asp.netCoreDatPhongKS.Models.ViewModels
{
 
    public class PhongViewModel
    {
        public Phong Phong { get; set; }
        public int SoDem { get; set; }
        public decimal TongGia => (Phong.GiaPhong1Dem ?? 0) * SoDem;
        public int SoKhach { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
    }
}
//xử lý tìm kiếm theo các điều của khách hàng
//khi sử dụng ViewModel:
//Tách biệt logic hiển thị: ViewModel giúp tách
//biệt các lớp dữ liệu của ứng dụng (Model) khỏi giao diện hiển thị, giúp dễ bảo trì và mở rộng.

//Dễ dàng thao tác với dữ liệu: Bạn có thể tính toán các giá trị (như tổng giá phòng) trong ViewModel
//thay vì làm điều đó trực tiếp trong view hoặc controller.

//Cải thiện bảo mật: Khi sử dụng ViewModel,
//bạn chỉ gửi những dữ liệu cần thiết đến view, tránh việc lộ thông tin không mong muốn.
