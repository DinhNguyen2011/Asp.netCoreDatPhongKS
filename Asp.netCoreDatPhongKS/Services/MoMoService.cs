using Asp.netCoreDatPhongKS.Models.Payment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<MoMoResponseModel> PaymentExecute(IQueryCollection query);
    }

    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MoMoService> _logger;

        public MoMoService(IConfiguration configuration, HttpClient httpClient, ILogger<MoMoService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            try
            {
                var partnerCode = _configuration["MoMo:PartnerCode"];
                var accessKey = _configuration["MoMo:AccessKey"];
                var secretKey = _configuration["MoMo:SecretKey"];
                var baseUrl = _configuration["MoMo:BaseUrl"];
                var returnUrl = _configuration["MoMo:ReturnUrl"];
                var notifyUrl = _configuration["MoMo:NotifyUrl"];

                var orderId = Guid.NewGuid().ToString();
                var requestId = Guid.NewGuid().ToString();
                var amount = ((long)model.Amount).ToString();
                var orderInfo = model.OrderDescription;
                var ipnUrl = notifyUrl;
                var redirectUrl = returnUrl;
                var extraData = "";

                var rawData = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=captureWallet";

                var signature = ComputeHmacSha256(rawData, secretKey);

                var requestBody = new
                {
                    partnerCode,
                    accessKey,
                    requestId,
                    amount,
                    orderId,
                    orderInfo,
                    redirectUrl,
                    ipnUrl,
                    extraData,
                    requestType = "captureWallet",
                    signature,
                    lang = "vi"
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("MoMo Request: {RequestBody}", json);

                var response = await _httpClient.PostAsync(baseUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("MoMo Response: {ResponseContent}", responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"MoMo API failed with status code {response.StatusCode}: {responseContent}");
                }

                var responseData = JsonSerializer.Deserialize<MoMoPaymentResponse>(responseContent);

                if (responseData == null)
                {
                    throw new Exception("MoMo API returned null response");
                }

                if (responseData.ResultCode != 0)
                {
                    throw new Exception($"MoMo API error: resultCode={responseData.ResultCode}, message={responseData.Message}");
                }

                if (string.IsNullOrEmpty(responseData.PayUrl))
                {
                    throw new Exception("MoMo API returned empty payUrl");
                }

                return responseData.PayUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating MoMo payment URL");
                throw;
            }
        }

        public Task<MoMoResponseModel> PaymentExecute(IQueryCollection query)
        {
            var secretKey = _configuration["MoMo:SecretKey"];
            var resultCode = query["resultCode"].ToString();
            var orderId = query["orderId"].ToString();
            var transId = query["transId"].ToString();

            var rawData = $"partnerCode={query["partnerCode"]}&orderId={orderId}&requestId={query["requestId"]}&amount={query["amount"]}&orderInfo={query["orderInfo"]}&orderType={query["orderType"]}&transId={transId}&resultCode={resultCode}&message={query["message"]}&payType={query["payType"]}&responseTime={query["responseTime"]}&extraData={query["extraData"]}";

            var signature = ComputeHmacSha256(rawData, secretKey);
            var isValidSignature = signature == query["signature"].ToString();

            return Task.FromResult(new MoMoResponseModel
            {
                Success = isValidSignature && resultCode == "0",
                OrderId = orderId,
                TransactionId = transId,
                ResponseCode = resultCode
            });
        }

        private string ComputeHmacSha256(string message, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(keyBytes);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var hash = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    public class MoMoPaymentResponse
    {
        public string PartnerCode { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string PayUrl { get; set; }
        public string Deeplink { get; set; }
        public string QrCodeUrl { get; set; }
    }

    public class MoMoResponseModel
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
    }
}