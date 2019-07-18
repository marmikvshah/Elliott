using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain
{
    public interface IFuturesAllocator
    {
        (API.Trade.Common.Trade Trade, string AllocatorMessage) AllocateFuture(API.Trade.Common.Trade Trade);
    }
}
