using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Trade.Common
{
    public class Trade
    {
        public Trade()
        {
            TradeId = -1;
            TradeAllocations = new List<TradeAllocation>();
        }


        [Column("TRADE_DATE")]
        public DateTime TradeDate { get; set; }
        [Column("TRADE_ID")]
        public long TradeId { get; set; }
        [Column("PRICE")]
        public double Price { get; set; }
        [Column("QUANTITY")]
        public double TradeQuantity { get; set; }
        [Column("TRADE_STATUS")]
        public string TradeStatus { get; set; }
        
        public Security Security { get; set; }

        [Column("TRADE_SIDE")]
        public string TradeSide { get; set; }
        public List<TradeAllocation> TradeAllocations { get; set; } = new List<TradeAllocation>();

        [Column("INSTRUMENT_ID")]
        public long InstrumentId { get; set; }

        public double AllocatedQuantity
        {
            get
            {
                return TradeAllocations.Sum(In => In.AllocationQuantity);
            }
        }





        public override string ToString()
        {
            StringBuilder StringBuilder = new StringBuilder();

            StringBuilder.AppendLine(string.Format("TradeId {0} Price {1} TradeQuantity {2} TradeStatus {3} SecurityIdentifier {4}  AssetType {5} TradeSide {6} AllocatedQuantity : {7}",
                TradeId, Price, TradeQuantity, TradeStatus, Security.SecurityIdentifier, Security.AssetType, TradeSide, AllocatedQuantity));

            StringBuilder.AppendLine("Trade Allocations");

            TradeAllocations.ForEach(In =>
            {
                StringBuilder.AppendLine(string.Format("AccountId : {0} AllocationQuantity : {1}", In.AccountId, In.AllocationQuantity));
            });

            return StringBuilder.ToString();
        }
    }
}
