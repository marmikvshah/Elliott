using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Create
{
    public class CreateTradeCommandResponses
    {
        public List<CreateTradeCommandResponse> Responses { get; private set; }
        public CreateTradeCommandResponses(List<CreateTradeCommandResponse> createTradeCommandResponses)
        {
            Responses = createTradeCommandResponses;
        }
    }
}
