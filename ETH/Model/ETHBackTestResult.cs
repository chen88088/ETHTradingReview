using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETH.Model
{
    public class ETHBackTestResult
    {
        public List<ETHTrade> Trades { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public decimal Profit { get; set; }
        public decimal ReturnOnInvestment { get; set; }
    }
}
