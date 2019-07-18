using API.Trade.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Create
{
    public class CreateTradeCommandResponse
    {
        public API.Trade.Common.Trade Trade { get; set; }
        public string RequestId { get; set; }
        public string RequestedBy { get; set; }
        public string RequestSource { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
        public string ResponseMessage { get; set; }
    }


}
