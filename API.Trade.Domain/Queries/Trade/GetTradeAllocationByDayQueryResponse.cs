using API.Trade.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Allocate
{
    public class GetTradeAllocationByDayQueryResponse
    {

        public List<TradeAllocationViewModel> Allocations { get; private set; }
        public GetTradeAllocationByDayQueryResponse(List<TradeAllocationViewModel> inViewModels)
        {
            Allocations = inViewModels;
        }
    }
}
