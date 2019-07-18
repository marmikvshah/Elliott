using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Create
{
    public class CreateTradeCommandRequests : ITradeRequest<CreateTradeCommandResponses>
    {
        public List<CreateTradeCommandRequest> Requests = new List<CreateTradeCommandRequest>();

        public override string ToString()
        {
            StringBuilder StringBuilder = new StringBuilder();
            StringBuilder.AppendLine("CreateTradeCommandRequests");
            Requests.ForEach(In => StringBuilder.AppendLine(In.ToString()));
            return StringBuilder.ToString();
        }
    }
}
