using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ETH.Model
{
    public class ETHApiClient
    {
        // 幣安交易所網址
        private string BaseUrl;

        // 個人金鑰
        private string ApiKey;
        private string SecretKey;

        public ETHApiClient(string baseUrl, string apiKey, string secretKey)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            SecretKey = secretKey;
        }


        public async Task<List<ETHCandleStick>> GetMarketDataAsync(string symbol, string interval, int limit)
        {
            var client = new HttpClient();
            var queryParams = $"?symbol={symbol}&interval={interval}&limit={limit}";
            var response = await client.GetAsync($"{BaseUrl}{queryParams}");

            if (response.IsSuccessStatusCode)
            {

                var json = await response.Content.ReadAsStringAsync();
                var ETHCandleSticks = JsonConvert.DeserializeObject<List<JArray>>(json);
                return ETHCandleSticks.Select(ETHCandleStick => new ETHCandleStick
                {
                    OpenTime = ETHCandleStick[0].ToObject<long>(),
                    OpenPrice = ETHCandleStick[1].ToObject<decimal>(),
                    HighPrice = ETHCandleStick[2].ToObject<decimal>(),
                    LowPrice = ETHCandleStick[3].ToObject<decimal>(),
                    ClosePrice = ETHCandleStick[4].ToObject<decimal>(),
                    Volume = ETHCandleStick[5].ToObject<decimal>(),
                    CloseTime = ETHCandleStick[6].ToObject<long>()
                }).ToList();
            }
            throw new Exception($"Failed to get klines. Status code: {response.StatusCode}");

        }
    }
}
