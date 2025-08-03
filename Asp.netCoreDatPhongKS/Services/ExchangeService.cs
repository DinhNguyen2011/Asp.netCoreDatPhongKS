using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Asp.netCoreDatPhongKS.Services
{
    public class ExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "2c2d5c86cd52468e86b48c8493c8aa9a";

        public ExchangeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> LayTyGiaUSDToVNDAsync()
        {
            var url = $"https://openexchangerates.org/api/latest.json?app_id={_apiKey}&symbols=VND";
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {json}"); // Log toàn bộ phản hồi
                var data = JsonSerializer.Deserialize<ExchangeRateResponse>(json);

                if (data != null && data.Rates != null && data.Rates.TryGetValue("VND", out decimal tyGia) && tyGia > 0)
                {
                    Console.WriteLine($"Tỷ giá USD/VND từ API: {tyGia}");
                    return tyGia;
                }
                Console.WriteLine("API không cung cấp tỷ giá VND hợp lệ, kiểm tra lại dữ liệu hoặc API key.");
                return 0m; // Báo hiệu lỗi
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Lỗi khi gọi API tỷ giá: {ex.Message}");
                return 0m; // Báo hiệu lỗi kết nối
            }
        }

        private class ExchangeRateResponse
        {
            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}