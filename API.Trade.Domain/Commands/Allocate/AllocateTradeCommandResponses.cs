using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Allocate
{
    public class AllocateTradeCommandResponses
    {
        public List<AllocateTradeCommandResponse> Responses { get; private set; }
        public AllocateTradeCommandResponses(List<AllocateTradeCommandResponse> createTradeCommandResponses)
        {
            Responses = createTradeCommandResponses;
        }
    }
}
