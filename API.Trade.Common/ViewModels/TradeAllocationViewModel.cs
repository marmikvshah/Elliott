using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Common.ViewModels
{
    public class TradeAllocationViewModel
    {
        public long TradeId { get; set; }
        public double TradeQuantity { get; set; }
        public double TradePrice { get; set; }
        
        public string TradeSide { get; set; }
        public string TradeStatus { get; set; }
        public DateTime TradeDate { get; set; }
        public long TradeAllocationId { get; set; }
        public string SecurityIdentifier { get; set; }

        public string AccountShotName { get; set; }
        public double AllocationQuantity { get; set; }
        public long AccountId { get; set; }
        
    }
}
