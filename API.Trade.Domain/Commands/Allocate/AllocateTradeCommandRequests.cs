using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Common;
using API.Trade.Shared.Messaging;

namespace API.Trade.Domain.Commands.Allocate
{
    public class AllocateTradeCommandRequests : ITradeRequest<AllocateTradeCommandResponses>
    {
        public List<AllocateTradeCommandRequest> Requests = new List<AllocateTradeCommandRequest>();

        public override string ToString()
        {
            StringBuilder StringBuilder = new StringBuilder();
            StringBuilder.AppendLine("AllocateTradeCommandRequests");
            Requests.ForEach(In => StringBuilder.AppendLine(In.ToString()));
            return StringBuilder.ToString();
        }
    }
}
