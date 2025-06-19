using Asp.netCoreDatPhongKS.Models.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Asp.netCoreDatPhongKS.Services
{
    public interface IMoMoService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext httpContext);
        PaymentResponse PaymentExecute(IQueryCollection query);
    }

    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MoMoService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext httpContext)
        {
            var endpoint = _configuration["MoMo:Endpoint"];
            var partnerCode = _configuration["MoMo:PartnerCode"];
            var accessKey = _configuration["MoMo:AccessKey"];
            var secretKey = _configuration["MoMo:SecretKey"];
            var returnUrl = _configuration["MoMo:ReturnUrl"];
            var notifyUrl = _configuration["MoMo:NotifyUrl"];
            var orderId = $"MM{DateTime.Now.Ticks}";
            var requestId = Guid.NewGuid().ToString();
            var amount = model.Amount.ToString("F0");
            var orderInfo = model.OrderDescription;
            var requestType = "captureWallet";
            var extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { PhieuDatPhongId = model.PhieuDatPhongId })));

            var rawSignature = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";
            var signature = ComputeHmacSha256(rawSignature, secretKey);

            var requestBody = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                requestType,
                signature,
                extraData,
                lang = "vi"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(endpoint, content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

            if (responseData.ContainsKey("payUrl"))
            {
                return responseData["payUrl"].ToString();
            }

            throw new Exception("Không thể tạo URL thanh toán MoMo: " + responseContent);
        }

        public PaymentResponse PaymentExecute(IQueryCollection query)
        {
            var secretKey = _configuration["MoMo:SecretKey"];
            var partnerCode = _configuration["MoMo:PartnerCode"];
            var accessKey = _configuration["MoMo:AccessKey"];
            var orderId = query["orderId"];
            var requestId = query["requestId"];
            var amount = query["amount"];
            var orderInfo = query["orderInfo"];
            var orderType = query["orderType"];
            var transId = query["transId"];
            var resultCode = query["resultCode"];
            var message = query["message"];
            var payType = query["payType"];
            var responseTime = query["responseTime"];
            var extraData = query["extraData"];
            var signature = query["signature"];

            var rawSignature = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&orderType={orderType}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
            var computedSignature = ComputeHmacSha256(rawSignature, secretKey);

            var response = new PaymentResponse
            {
                OrderId = orderId,
                TransactionId = transId,
                Amount = decimal.TryParse(amount, out var amt) ? amt : 0,
                Success = resultCode == "0" && computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase),
                PaymentMethod = "MoMo",
                MoMoResponseCode = resultCode,
                OrderDescription = orderInfo,
                VnPayResponseCode = null, // Không dùng cho MoMo
                Token = signature
            };

            return response;
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}