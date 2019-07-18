using API.Trade.Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Create
{
    public class CreateTradeCommandRequest : ITradeRequest<CreateTradeCommandResponse>
    {
        public API.Trade.Common.Trade Trade { get; set; }
        public string RequestId { get; set; }
        public string RequestedBy { get; set; }
        public string RequestSource { get; set; }

        public override string ToString()
        {
            StringBuilder StringBuilder = new StringBuilder();
            StringBuilder.AppendLine(string.Format("TradeId {0} TradeSide {1} TradeQuantity {2}", Trade.TradeId, Trade.TradeStatus, Trade.TradeQuantity));

            Trade
                .TradeAllocations
                .ForEach(In =>
                {
                    StringBuilder.AppendLine(string.Format("TradeAllocationId : {0} TradeAllocationQuantity {1} Account Name  '{2}'",
                        In.TradeAllocationId, In.AllocationQuantity, In.AccountId));
                }
                );

            return StringBuilder.ToString();
        }
    }


}
