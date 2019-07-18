using API.Trade.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Allocate
{
    public class GetTradeStatusQueryResponse
    {
        public API.Trade.Common.Trade Trade { get; private set; }
        public GetTradeStatusQueryResponse(API.Trade.Common.Trade inTrade)
        {
            Trade = inTrade;
        }
    }
}
