using API.Trade.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain
{
    public interface IEquityAllocator  
    {
        (API.Trade.Common.Trade Trade,string AllocatorMessage)  AllocateEquity(API.Trade.Common.Trade Trade, List<API.Trade.Common.Trade> ExistingTrades);
    }
}
