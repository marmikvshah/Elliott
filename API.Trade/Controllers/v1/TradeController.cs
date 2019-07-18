using API.Trade.Domain.Commands.Allocate;
using API.Trade.Domain.Commands.Create;
using API.Trade.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Trade.Controllers.v1
{

    [ApiVersion("1")]
    [Authorize]
    public class TradeController : Controller
    {
        private readonly ITradeMessaging _TradeMessaging;
        public TradeController(ITradeMessaging tradeMessaging)
        {
            _TradeMessaging = tradeMessaging;
        }

        [HttpPut]
        [Route("CreateTrades")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<CreateTradeCommandResponses> CreateTrades(CreateTradeCommandRequests createTradeRequests)
        {
            CreateTradeCommandResponses CreateTradeCommandResponses = await _TradeMessaging.Send(createTradeRequests);
            return CreateTradeCommandResponses;
        }

        [HttpPut]
        [Route("AllocateTrades")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<AllocateTradeCommandResponses> AllocateTrades(AllocateTradeCommandRequests allocateTradeCommandRequests)
        {
            AllocateTradeCommandResponses AllocateTradeCommandResponses = await _TradeMessaging.Send(allocateTradeCommandRequests);
            return AllocateTradeCommandResponses;
        }

        [HttpGet]
        [Route("CheckTradeAllocationStatus")]
        [ProducesResponseType(typeof(GetTradeStatusQueryResponse), StatusCodes.Status200OK)]
        public async Task<GetTradeStatusQueryResponse> CheckTradeAllocationStatus(GetTradeStatusQueryRequest getTradeStatusQueryRequest)
        {
            GetTradeStatusQueryResponse getTradeStatusQueryResponse = await _TradeMessaging.Send(getTradeStatusQueryRequest);
            return getTradeStatusQueryResponse;
        }

        [HttpGet]
        [Route("GetAllTradeAllocationByDate")]
        [ProducesResponseType(typeof(GetTradeAllocationByDayQueryResponse), StatusCodes.Status200OK)]
        public async Task<GetTradeAllocationByDayQueryResponse> GetAllTradeAllocationByDate(GetTradeAllocationByDayQueryRequest getTradeAllocationByDayQueryRequest)
        {
            GetTradeAllocationByDayQueryResponse getTradeAllocationByDayQueryResponse = await _TradeMessaging.Send(getTradeAllocationByDayQueryRequest);
            return getTradeAllocationByDayQueryResponse;
        }
    }
}