using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHLibrary
{
    /// <summary>
    /// 是 ETHBacktester 类的回测结果类，用于存储回测的结果信息，包括所有交易的详细信息、账户余额、总利润和投资回报率等信息。
    /// 在 ETHBacktestResult 中，你可以方便地获取交易次数、平均获利和亏损交易的数量等统计信息，以便更好地评估你的交易策略的性能。
    /// </summary>
    public class ETHBackTestResult
    {
        public List<ETHTrade> Trades { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public decimal Profit { get; set; }
        public decimal ReturnOnInvestment { get; set; }
    }
}
