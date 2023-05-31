using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ETHLibrary
{
    public class ETHApiClient
    {
        // 幣安交易所網址
        private string BaseUrl ;

        // 個人金鑰
        private string ApiKey ;
        private string SecretKey ;

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

        /// <summary>
        /// 方法--使用Binance API獲取K線數據
        /// </summary>
        /// <param name="symbol">交易對</param>
        /// <param name="interval">時間間隔</param>
        /// <param name="limit">K線數量的上限</param>
        /// <returns></returns>
        public List<ETHCandleStick> GetCandlestickData(string symbol, string interval, int limit)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("/api/v3/klines", Method.Get);

            request.AddParameter("symbol", symbol);
            request.AddParameter("interval", interval);
            request.AddParameter("limit", limit);

            var response = client.Execute(request);
            var content = response.Content;

            var jArray = JArray.Parse(content);

            var candlesticks = new List<ETHCandleStick>();

            foreach (var item in jArray)
            {
                candlesticks.Add(new ETHCandleStick
                {
                    OpenTime = Convert.ToInt64(UnixTimeStampToDateTime((long)item[6])),
                    OpenPrice = (decimal)item[1],
                    HighPrice = (decimal)item[2],
                    LowPrice = (decimal)item[3],
                    ClosePrice = (decimal)item[4],
                    Volume = (decimal)item[5],
                    CloseTime = Convert.ToInt64(UnixTimeStampToDateTime((long)item[6]))
                });
            }

            return candlesticks;
        }

        /// <summary>
        /// 使用 DateTimeOffset.FromUnixTimeMilliseconds 方法將 Unix 時間戳轉換為 DateTimeOffset，然後再使用 LocalDateTime 屬性獲取轉換後的本機時間。
        /// </summary>
        /// <param name="unixTimeStamp"> Unix 時間戳記</param>
        /// <returns>轉換後的本機時間</returns>
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp);
            return dateTimeOffset.LocalDateTime;
        }

    }
}
