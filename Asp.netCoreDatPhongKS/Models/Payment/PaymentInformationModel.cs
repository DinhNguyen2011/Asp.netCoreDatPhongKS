namespace Asp.netCoreDatPhongKS.Models.Payment
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; } = "hotel_booking";
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public int PhieuDatPhongId { get; set; } // Liên kết với phiếu đặt phòng
    }
}
