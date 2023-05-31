using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETH.Model;

using Microsoft.AspNetCore.Mvc;



namespace ETH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ETHController : ControllerBase
    {
        // GET api/values
        [HttpGet("reviewTest/{symbol}/{interval}/{limit}")]
        public async Task<string> ExecuteReviewTestAsync([FromQuery]  string symbol, string interval, int limit)
        {
            symbol = "ETHUSDT";

            // 幣安交易所網址 & 私人api金鑰
            string baseUrl = "https://api.binance.com/api/v3/klines";
            string apiKey = "";
            string secretKey = "";

            // 建立索取資料對象
            ETHApiClient ETHHistoryDataCollector = new ETHApiClient(baseUrl, apiKey, secretKey);

            // 向交易所索取指定時間區間的交易資料
            List<ETHCandleStick> candleStickData = await ETHHistoryDataCollector.GetMarketDataAsync(symbol, interval, limit);

            decimal Balance = 10000m;
            // 執行回測
            ETHBackTester reviewTester = new ETHBackTester(symbol, Balance, candleStickData);
            ETHBackTestResult testResult = reviewTester.Run();
            return testResult.ReturnOnInvestment.ToString();
        }

        
        [HttpGet("test/{symbol}/{interval}/{limit}")]
        public async Task<string> GetCurrentETHDataAsync([FromQuery]string symbol, string interval, int limit)
        {
            symbol = "ETHUSDT";

            // 幣安交易所網址 & 私人api金鑰
            string baseUrl = "https://api.binance.com/api/v3/klines";
            string apiKey = "";
            string secretKey = "";

            // 建立索取資料對象
            ETHApiClient ETHCurrentDataCollector = new ETHApiClient(baseUrl, apiKey, secretKey);
            var ETHData = await ETHCurrentDataCollector.GetMarketDataAsync(symbol, interval, limit);

            return ETHData[0].HighPrice.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
