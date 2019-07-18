using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Common;
using API.Trade.Shared.Messaging;

namespace API.Trade.Domain.Commands.Allocate
{
    public class GetTradeAllocationByDayQueryRequest : ITradeRequest<GetTradeAllocationByDayQueryResponse>
    {

        public List<DateTime> RequestedDates = new List<DateTime>();
        public string RequestId { get; set; }
        public string RequestedBy { get; set; }
        public string RequestSource { get; set; }
    }
}
