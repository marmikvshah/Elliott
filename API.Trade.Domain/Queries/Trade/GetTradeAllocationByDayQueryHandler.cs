using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API.Trade.Common;
using API.Trade.Common.ViewModels;
using API.Trade.Data.Queries.Interface;
using API.Trade.Shared;
using API.Trade.Shared.Logger;
using API.Trade.Shared.Messaging;

namespace API.Trade.Domain.Commands.Allocate
{
    public class GetTradeAllocationByDayQueryHandler : ITradeRequestHandler<GetTradeAllocationByDayQueryRequest, GetTradeAllocationByDayQueryResponse>
    {

        private readonly ITradeMessaging _messaging;
        private readonly ITradeGetRepository _tradeGetRepository;
        private readonly ILogger _Logger;

        public GetTradeAllocationByDayQueryHandler(
                ITradeMessaging messaging,
                ITradeGetRepository tradeGetRepository, ILogger Logger)
        {
            _messaging = messaging;
            _tradeGetRepository = tradeGetRepository;
            _Logger = Logger;
        }

        public async Task<GetTradeAllocationByDayQueryResponse> Handle(GetTradeAllocationByDayQueryRequest request, CancellationToken cancellationToken)
        {
            List<API.Trade.Common.Trade> Trades = await _tradeGetRepository.GetTradeAllocationByDates(request.RequestedDates);
            List<TradeAllocationViewModel> TradeAllocations = Trades
                .SelectMany(Trade => Trade.TradeAllocations
                .Select(TradeAllocation =>
                new TradeAllocationViewModel
                {
                    TradeId = Trade.TradeId,
                    TradePrice = Trade.Price,
                    TradeQuantity = Trade.TradeQuantity,
                    TradeSide = Trade.TradeSide,
                    TradeStatus = Trade.TradeStatus,
                    TradeDate = Trade.TradeDate,
                    TradeAllocationId = TradeAllocation.TradeAllocationId,
                    SecurityIdentifier = Trade.Security.SecurityIdentifier,
                    AccountShotName = TradeAllocation.AccountShotName,
                    AllocationQuantity = TradeAllocation.AllocationQuantity,
                    AccountId = TradeAllocation.AccountId,
                })).ToList();

            GetTradeAllocationByDayQueryResponse GetTradeAllocationByDayQueryResponse = new GetTradeAllocationByDayQueryResponse(TradeAllocations);
            return GetTradeAllocationByDayQueryResponse;
        }
    }
}
