using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETH.Model
{
    public class ETHBackTester
    {
        /// <summary>
        /// 手續費費率，目前設置為 0.001
        /// </summary>
        private const decimal TradingFeeRate = 0.001m;

        /// <summary>
        /// 交易對的名稱，如 BTCUSDT。
        /// </summary>
        private readonly string _symbol;

        /// <summary>
        /// 回測開始時的賬戶餘額
        /// </summary>
        private readonly decimal _startingBalance;

        /// <summary>
        /// 用來回測的 K 線數據
        /// </summary>
        private readonly List<ETHCandleStick> _candlesticks;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="symbol">交易對的名稱</param>
        /// <param name="startingBalance">回測開始時的賬戶餘額</param>
        /// <param name="candlesticks">用來回測的 K 線數據</param>
        public ETHBackTester(string symbol, decimal startingBalance, List<ETHCandleStick> candlesticks)
        {
            _symbol = symbol;
            _startingBalance = startingBalance;
            _candlesticks = candlesticks;
        }

        /// <summary>
        /// 執行回測(含交易策略)。
        /// </summary>
        /// <returns>返回一個 BinanceBacktestResult 對象，該對象包含了回測結果</returns>
        public ETHBackTestResult Run()
        {
            // 交易記錄列表，用來記錄每次交易的詳細信息
            var trades = new List<ETHTrade>();

            // 回測期間賬戶的餘額，初始值為 _startingBalance
            var balance = _startingBalance;

            // 當前持倉量，初始值為 0
            var position = 0m;

            // 上一個 K 線數據對象，在迴圈中用來判斷趨勢
            var lastCandlestick = _candlesticks.First();

            foreach (var candlestick in _candlesticks.Skip(1))
            {
                // 當前市場價格，在迴圈中根據 K 線數據計算得到
                var currentPrice = candlestick.ClosePrice;

                // 交易策略--買進邏輯
                // If the current price is higher than the previous price, buy
                if ((currentPrice > lastCandlestick.ClosePrice) && (position == 0m))
                {
                    // 買入或賣出的數量，根據賬戶餘額和市場價格計算得到
                    var quantity = balance / currentPrice;

                    // 更新持倉量
                    position = quantity;

                    // 本次交易產生的手續費，根據 TradingFeeRate、數量和價格計算得到。
                    var tradingFee = TradingFeeRate * quantity * currentPrice;

                    position = Math.Round(position, 3, MidpointRounding.ToEven);
                    currentPrice = Math.Round(currentPrice, 3, MidpointRounding.ToEven);
                    tradingFee = Math.Round(tradingFee, 3, MidpointRounding.ToEven);
                    balance = Math.Round(balance, 3, MidpointRounding.ToEven);

                    decimal value = Math.Round((position * currentPrice), 3, MidpointRounding.ToEven);

                    // 每次交易帳戶餘額都得扣除交易手續費
                    balance = (balance - value) - tradingFee;

                    //新增交易
                    trades.Add(new ETHTrade
                    {
                        Symbol = _symbol,
                        Price = currentPrice,
                        Quantity = quantity,
                        IsBuy = true,
                        Timestamp = candlestick.CloseTime
                    });
                }

                // 交易策略--賣出邏輯
                // If the current price is lower than the previous price, sell
                if ((currentPrice < lastCandlestick.ClosePrice) && (position > 0m))
                {
                    var tradingFee = TradingFeeRate * position * currentPrice;

                    position = Math.Round(position, 3, MidpointRounding.ToEven);
                    currentPrice = Math.Round(currentPrice, 3, MidpointRounding.ToEven);
                    tradingFee = Math.Round(tradingFee, 3, MidpointRounding.ToEven);
                    balance = Math.Round(balance, 3, MidpointRounding.ToEven);

                    decimal value = Math.Round((position * currentPrice - tradingFee), 3, MidpointRounding.ToEven);
                    balance = balance + value;
                    position = 0m;
                    trades.Add(new ETHTrade
                    {
                        Symbol = _symbol,
                        Price = currentPrice,
                        Quantity = position,
                        IsBuy = false,
                        Timestamp = candlestick.CloseTime
                    });
                }

                lastCandlestick = candlestick;
            }

            // Sell any remaining position
            if (position > 0m)
            {
                var currentPrice = lastCandlestick.ClosePrice;
                var tradingFee = TradingFeeRate * position * currentPrice;



                balance += Math.Round((position * currentPrice - tradingFee), 3, MidpointRounding.ToEven);
                trades.Add(new ETHTrade
                {
                    Symbol = _symbol,
                    Price = currentPrice,
                    Quantity = position,
                    IsBuy = false,
                    Timestamp = lastCandlestick.CloseTime
                });
            }

            // 回測結束時的賬戶餘額，計算方法是最後一次交易後的餘額
            var endingBalance = balance;

            // 回測期間的收益，計算方法是 endingBalance - _startingBalance。
            var profit = endingBalance - _startingBalance;

            // 回測期間的投資回報率，計算方法是 (profit / _startingBalance) * 100
            var returnOnInvestment = profit / _startingBalance * 100m;

            return new ETHBackTestResult
            {
                Trades = trades,
                StartingBalance = _startingBalance,
                EndingBalance = endingBalance,
                Profit = profit,
                ReturnOnInvestment = returnOnInvestment
            };
        }
    }
}
