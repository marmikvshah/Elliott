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
    public class GetTradeStatusQueryHandler : ITradeRequestHandler<GetTradeStatusQueryRequest, GetTradeStatusQueryResponse>
    {

        private readonly ITradeMessaging _messaging;
        private readonly ITradeGetRepository _tradeGetRepository;
        private readonly ILogger _Logger;

        public GetTradeStatusQueryHandler(
                ITradeMessaging messaging,
                ITradeGetRepository tradeGetRepository, ILogger Logger)
        {
            _messaging = messaging;
            _tradeGetRepository = tradeGetRepository;
            _Logger = Logger;
        }

        public async Task<GetTradeStatusQueryResponse> Handle(GetTradeStatusQueryRequest request, CancellationToken cancellationToken)
        {

            API.Trade.Common.Trade Trade = await _tradeGetRepository.GetTrade(request.Trade.TradeId);
            GetTradeStatusQueryResponse getTradeStatusQueryResponse = new GetTradeStatusQueryResponse(Trade);
            return getTradeStatusQueryResponse;
        }
    }
}
