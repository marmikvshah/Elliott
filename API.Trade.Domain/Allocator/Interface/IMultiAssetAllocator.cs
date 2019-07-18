using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain
{
    public interface IMultiAssetAllocator : IEquityAllocator, IFuturesAllocator { }
}
