namespace Asp.netCoreDatPhongKS.Models.Payment
{
    public class MoMoPaymentResponse
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string MoMoResponseCode { get; set; }
        public string Message { get; set; }
        public int PhieuDatPhongId { get; set; }
    }
}