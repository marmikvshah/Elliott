using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API.Trade.Common
{
    public class TradeAllocation
    {
        [Column("TRADE_ALLOCATION_ID")]
        public long TradeAllocationId { get; set; }
        [Column("TRADE_ID")]
        public long TradeId { get; set; }
        [Column("ACCOUNT_ID")]
        public long AccountId { get; set; }
        [Column("ACCOUNT_SHORT_NAME")]
        public string AccountShotName { get; set; }
        [Column("ALLOCATION_QUANTITY")]
        public double AllocationQuantity { get; set; }
    }

    public class TradeInstrument
    {
        public long TradeId { get; set; }
        public long InstrumentId { get; set; }
    }
}
