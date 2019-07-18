using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Common;
using API.Trade.Shared.Messaging;

namespace API.Trade.Domain.Commands.Allocate
{
    public class GetTradeStatusQueryRequest : ITradeRequest<GetTradeStatusQueryResponse>
    {
        public API.Trade.Common.Trade Trade { get; set; }
    }
}
