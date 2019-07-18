using API.Trade.Common;
using API.Trade.Data.Commands.Repositories.Interfaces;
using API.Trade.Data.Queries.Interface;
using API.Trade.Shared;
using API.Trade.Shared.Logger;
using API.Trade.Shared.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Trade.Domain.Commands.Create
{
    public class CreateTradeCommandHandler : ITradeRequestHandler<CreateTradeCommandRequests, CreateTradeCommandResponses>
    {

        private readonly ITradeMessaging _messaging;
        private readonly ILogger _Logger;
        private readonly ITradeCreateRepository _tradeCreateRepository;
        private readonly ITradeGetRepository _tradeGetRepository;
        private readonly ISecurityRepository _securityRepository;
        private readonly IAccountRepository _accountRepository;


        public CreateTradeCommandHandler(
                ITradeMessaging messaging,
                ITradeCreateRepository tradeCreateRepository,
                ITradeGetRepository tradeGetRepository,
                ISecurityRepository securityRepository,
                IAccountRepository accountRepository,
                ILogger Logger)
        {
            _tradeCreateRepository = tradeCreateRepository;
            _tradeGetRepository = tradeGetRepository;
            _securityRepository = securityRepository;
            _accountRepository = accountRepository;
            _messaging = messaging;
            _Logger = Logger;
        }


        public async Task<CreateTradeCommandResponses> Handle(CreateTradeCommandRequests requests, CancellationToken cancellationToken)
        {

            List<CreateTradeCommandResponse> createTradeCommandResponses = new List<CreateTradeCommandResponse>();

            _Logger.Debug(requests.ToString());

            foreach (CreateTradeCommandRequest createTradeCommandRequest in requests.Requests)
            {
                API.Trade.Common.Trade NewTrade = new Common.Trade();
                NewTrade.Price = createTradeCommandRequest.Trade.Price;
                NewTrade.TradeQuantity = createTradeCommandRequest.Trade.TradeQuantity;
                NewTrade.TradeSide = createTradeCommandRequest.Trade.TradeSide;
                NewTrade.TradeStatus = "UNALLOCATED";

                Security Security = await _securityRepository.GetSecurity(createTradeCommandRequest.Trade.Security.SecurityIdentifier);
                NewTrade.Security = Security;

                var result = await _tradeCreateRepository.CreateTrade(NewTrade);

                CreateTradeCommandResponse createTradeResponse = new CreateTradeCommandResponse();
                createTradeResponse.ResponseMessage = result.Message;

                if (result.Success)
                {
                    createTradeResponse.ResponseStatus = ResponseStatus.Success;
                    createTradeResponse.ResponseMessage = "Trade Successfully Created.";
                }
                else
                {
                    createTradeResponse.ResponseStatus = ResponseStatus.Failed;
                    createTradeResponse.ResponseMessage = "Trade Failed to Create.";
                }
                createTradeCommandResponses.Add(createTradeResponse);
            }

            CreateTradeCommandResponses _createTradeCommandResponses = new CreateTradeCommandResponses(createTradeCommandResponses);

            _Logger.Debug(_createTradeCommandResponses.ToString());

            return _createTradeCommandResponses;
        }
    }
}
