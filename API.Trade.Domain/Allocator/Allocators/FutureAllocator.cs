using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Common;

namespace API.Trade.Domain
{
    internal class FutureAllocator : IFuturesAllocator
    {
        public (Common.Trade Trade, string AllocatorMessage) AllocateFuture(Common.Trade trade)
        {

            List<Account> _adjustedAccounts = new List<Account>();

            StringBuilder StringBuilder = new StringBuilder();

            StringBuilder.AppendLine("Before Allocations.");
            StringBuilder.AppendLine(trade.ToString());

            TradeAllocation TradeAllocationA = trade.TradeAllocations.Find(In => In.AccountShotName == "A");
            TradeAllocation TradeAllocationB = trade.TradeAllocations.Find(In => In.AccountShotName == "B");

            double RemainingQuantity = trade.TradeQuantity;
            if (TradeAllocationA != null)
            {
                TradeAllocationA.AllocationQuantity = Math.Round(RemainingQuantity * 0.30, 0);
                RemainingQuantity = -TradeAllocationA.AllocationQuantity;
            }

            if (TradeAllocationB != null)
            {
                TradeAllocationB.AllocationQuantity = RemainingQuantity;
                RemainingQuantity = -TradeAllocationB.AllocationQuantity;
            }

            StringBuilder.AppendLine("After Allocations.");
            StringBuilder.AppendLine(trade.ToString());

            return (trade, StringBuilder.ToString());
        }
    }
}
