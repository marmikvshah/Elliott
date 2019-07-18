using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Common;
using Microsoft.Extensions.Caching.Memory;

namespace API.Trade.Domain
{
    public class MultiAssetAllocator : IMultiAssetAllocator
    {

        private readonly IEquityAllocator _equityCalculator;
        private readonly IFuturesAllocator _futureCalculator;

        public MultiAssetAllocator(IEquityAllocator equityAllocator, IFuturesAllocator futuresAllocator, IMemoryCache cache)
        {
            _equityCalculator = equityAllocator;
            _futureCalculator = futuresAllocator;
        }

        public (Common.Trade Trade, string AllocatorMessage) AllocateEquity(Common.Trade trade, List<Common.Trade> existingTrades)
        {
            return _equityCalculator.AllocateEquity(trade, existingTrades);
        }

        public (Common.Trade Trade, string AllocatorMessage) AllocateFuture(Common.Trade Trade)
        {
            return _futureCalculator.AllocateFuture(Trade);
        }
    }
}
