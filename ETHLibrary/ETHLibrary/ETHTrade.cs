using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHLibrary
{
    public class ETHTrade
    {
        // 代表交易的幣種對
        public string Symbol { get; set; }

        // 代表交易時的價格
        public decimal Price { get; set; }

        // 代表交易的數量
        public decimal Quantity { get; set; }

        // 代表交易類型,如果是買入，則值為 true，否則(賣出)為 false
        public bool IsBuy { get; set; }

        // 代表交易發生的時間，是一個長整數類型的屬性
        public long Timestamp { get; set; }
    }
}
