using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API.Trade.Common
{
    public class Account
    {

        [Column("ACCOUNT_SHORT_NAME")]
        public string AccountShortName { get; set; }
        [Column("ACCOUNT_ID")]
        public long AccountId { get; set; }
        [Column("ACCOUNT_LONG_NAME")]
        public string AccountLongName { get; set; }

        [Column("ACCOUNT_NAV")]
        public double NAV { get; set; }
        [Column("BUSINESS_DATE")]
        public DateTime ValueDate { get; set; }

        public void AdjustNAV(Trade trade, TradeAllocation tradeAllocation)
        {

        }
    }
}
