using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API.Trade.Common;
using API.Trade.Data.Commands.Repositories.Interfaces;
using API.Trade.Shared;
using API.Trade.Shared.Logger;
using API.Trade.Shared.Messaging;
using API.Trade.Data.Queries.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace API.Trade.Domain.Commands.Allocate
{
    public class AllocateTradeCommandHandler : ITradeRequestHandler<AllocateTradeCommandRequests, AllocateTradeCommandResponses>
    {

        private readonly ITradeMessaging _messaging;
        private readonly ILogger _Logger;
        private readonly ITradeCreateRepository _tradeCreateRepository;
        private readonly ITradeGetRepository _tradeGetRepository;
        private readonly ISecurityRepository _securityRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMultiAssetAllocator _multiAssetAllocator;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(1);

        public AllocateTradeCommandHandler(
                ITradeMessaging messaging,
                ITradeCreateRepository tradeCreateRepository,
                ITradeGetRepository tradeGetRepository,
                ISecurityRepository securityRepository,
                IAccountRepository accountRepository,
                ILogger Logger,
                IMemoryCache cache,
                IMultiAssetAllocator multiAssetAllocator
                )
        {
            _tradeCreateRepository = tradeCreateRepository;
            _tradeGetRepository = tradeGetRepository;
            _securityRepository = securityRepository;
            _accountRepository = accountRepository;
            _messaging = messaging;
            _Logger = Logger;
            _cache = cache;
            _multiAssetAllocator = multiAssetAllocator;
        }

        public async Task<AllocateTradeCommandResponses> Handle(AllocateTradeCommandRequests requests, CancellationToken cancellationToken)
        {

            List<AllocateTradeCommandResponse> responses = new List<AllocateTradeCommandResponse>();
            List<Account> AccountsNAV = (List<Account>)_cache.Get("AccountsNAV");
            if (AccountsNAV is null)
            {
                AccountsNAV = await _accountRepository.GetAccounts();
                _cache.Set("AccountsNAV", AccountsNAV, DefaultExpiration);
            }

            _Logger.Debug(requests.ToString());

            foreach (AllocateTradeCommandRequest allocateTradeCommandRequest in requests.Requests)
            {
                AllocateTradeCommandResponse allocateTradeCommandResponse = new AllocateTradeCommandResponse();

                API.Trade.Common.Trade ExistingTrade = await _tradeGetRepository.GetTrade(allocateTradeCommandRequest.Trade.TradeId);

                if (ExistingTrade.TradeStatus != "UNALLOCATED")
                {
                    allocateTradeCommandResponse.ResponseStatus = ResponseStatus.Failed;
                    allocateTradeCommandResponse.ResponseMessage = "Trade is no longer in Valid status for Allocation.";
                }
                else
                {
                    allocateTradeCommandResponse.Trade = ExistingTrade;

                    ExistingTrade.Price = allocateTradeCommandRequest.Trade.Price;
                    ExistingTrade.TradeQuantity = allocateTradeCommandRequest.Trade.TradeQuantity;
                    ExistingTrade.TradeSide = allocateTradeCommandRequest.Trade.TradeSide;

                    AccountsNAV
                        .ForEach(In =>
                        {
                            ExistingTrade.TradeAllocations.Add(new TradeAllocation
                            {
                                AccountId = In.AccountId,
                                AllocationQuantity = 0,
                                TradeId = ExistingTrade.TradeId,
                                AccountShotName = In.AccountShortName
                            }); ;
                        });

                    if (ExistingTrade.Security.AssetType == AssetTypes.Equity)
                    {

                        List<Common.Trade> AllDayTradesforSecurity = await _tradeGetRepository
                                        .GetAllTradesBySecyrityIdByDate(ExistingTrade.Security.SecurityIdentifier, ExistingTrade.TradeDate);

                        ExistingTrade.TradeStatus = "ALLOCATED";
                        var AllocatorResponse = _multiAssetAllocator.AllocateEquity(ExistingTrade, AllDayTradesforSecurity);
                        allocateTradeCommandResponse.ResponseStatus = ResponseStatus.Success;
                        allocateTradeCommandResponse.ResponseMessage = AllocatorResponse.AllocatorMessage;
                    }
                    else if (ExistingTrade.Security.AssetType == AssetTypes.Futures)
                    {
                        ExistingTrade.TradeStatus = "ALLOCATED";
                        var AllocatorResponse = _multiAssetAllocator.AllocateFuture(ExistingTrade);
                        allocateTradeCommandResponse.ResponseStatus = ResponseStatus.Success;
                        allocateTradeCommandResponse.ResponseMessage = AllocatorResponse.AllocatorMessage;
                    }
                    else
                    {
                        ExistingTrade.TradeStatus = "UNALLOCATED";
                        allocateTradeCommandResponse.ResponseStatus = ResponseStatus.Failed;
                        allocateTradeCommandResponse.ResponseMessage = "Asset Type could not be identified.";
                    }
                    responses.Add(allocateTradeCommandResponse);
                }
            }

            AllocateTradeCommandResponses allocateTradeCommandResponses = new AllocateTradeCommandResponses(responses);
            _Logger.Debug(allocateTradeCommandResponses.ToString());

            return allocateTradeCommandResponses;
        }
    }
}
