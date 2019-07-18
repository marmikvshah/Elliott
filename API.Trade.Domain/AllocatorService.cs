/*using API.Trade.Allocator;
using API.Trade.DataAccssLayer;
using API.Trade.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace API.Trade.Domain
{
    public class AllocatorService : IAllocatorService
    {

        private readonly ITradeRepository _TradeRepository = null;
        private readonly IMultiAssetAllocator _allocator = null;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(1);

        public AllocatorService(ITradeRepository tradeRepository, IMultiAssetAllocator allocator, IMemoryCache cache)
        {
            _TradeRepository = tradeRepository;
            _allocator = allocator;
            _cache = cache;
        }
        public async Task<List<CreateTradeResponse>> CreateTrades(List<CreateTradeRequest> createTradeRequests)
        {

            List<CreateTradeResponse> createTradeResponses = new List<CreateTradeResponse>();


            foreach (CreateTradeRequest createTradeRequest in createTradeRequests)
            {
                API.Trade.Model.Trade NewTrade = new Model.Trade();
                NewTrade.Price = createTradeRequest.Trade.Price;
                NewTrade.TradeQuantity = createTradeRequest.Trade.TradeQuantity;
                NewTrade.TradeSide = createTradeRequest.Trade.TradeSide;
                NewTrade.TradeStatus = "UNALLOCATED";

                Security Security = await _TradeRepository.GetSecurity(createTradeRequest.Trade.Security.SecurityIdentifier);
                NewTrade.Security = Security;

                var result = await _TradeRepository.SaveTrade(NewTrade);

                CreateTradeResponse createTradeResponse = new CreateTradeResponse();
                //createTradeResponse.OriginalRequest = createTradeRequest;
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
                createTradeResponses.Add(createTradeResponse);
            }
            return createTradeResponses;
        }

        public async Task<List<AllocateTradeResponse>> AllocateTrades(List<AllocateTradeRequest> allocateTradeRequests)
        {

            List<Account> AccountsNAV = (List<Account>)_cache.Get("AccountsNAV");
            if (AccountsNAV is null)
            {
                AccountsNAV = await _TradeRepository.GetAccounts();
                _cache.Set("AccountsNAV", AccountsNAV, DefaultExpiration);
            }

            List<AllocateTradeResponse> allocateTradeResponses = new List<AllocateTradeResponse>();

            foreach (AllocateTradeRequest allocateTradeRequest in allocateTradeRequests)
            {

                AllocateTradeResponse allocateTradeResponse = new AllocateTradeResponse();
                allocateTradeResponse.OriginalRequest = allocateTradeRequest;

                //Get Complete Security
                Security Security = await _TradeRepository.GetSecurity(allocateTradeRequest.Trade.Security.SecurityIdentifier);

                if (Security != null)
                {
                    API.Trade.Model.Trade ExistingTrade = await _TradeRepository.GetTrade(allocateTradeRequest.Trade.TradeId);
                    if (ExistingTrade.TradeStatus != "UNALLOCATED")
                    {
                        allocateTradeResponse.ResponseStatus = ResponseStatus.Failed;
                        allocateTradeResponse.ResponseMessage = "Trade is no longer in Valid status for Allocation.";
                    }
                    else
                    {
                        ((AllocateTradeRequest)allocateTradeResponse.OriginalRequest).Trade = ExistingTrade;

                        ExistingTrade.Price = allocateTradeRequest.Trade.Price;
                        ExistingTrade.TradeQuantity = allocateTradeRequest.Trade.TradeQuantity;
                        ExistingTrade.TradeSide = allocateTradeRequest.Trade.TradeSide;

                        //List<Account> Accounts = await _TradeRepository.GetAccounts();
                        AccountsNAV.ForEach(In =>
                        {
                            ExistingTrade.TradeAllocations.Add(new TradeAllocation
                            {
                                AccountId = In.AccountId,
                                AllocationQuantity = 0,
                                TradeId = ExistingTrade.TradeId,
                                AccountShotName = In.AccountShortName
                            }); ;
                        });

                        (Model.Trade Trade, string AllocatorMessage) AllocatorResponse;

                        if (ExistingTrade.Security.AssetType == AssetTypes.Equity)
                        {
                            ExistingTrade.TradeStatus = "ALLOCATED";
                            AllocatorResponse = _allocator.AllocateEquity(ExistingTrade, null);
                            allocateTradeResponse.ResponseStatus = ResponseStatus.Success;
                            allocateTradeResponse.ResponseMessage = AllocatorResponse.AllocatorMessage;



                        }
                        else if (ExistingTrade.Security.AssetType == AssetTypes.Futures)
                        {
                            ExistingTrade.TradeStatus = "ALLOCATED";
                            AllocatorResponse = _allocator.AllocateFuture(ExistingTrade);
                            allocateTradeResponse.ResponseStatus = ResponseStatus.Success;
                            allocateTradeResponse.ResponseMessage = AllocatorResponse.AllocatorMessage;
                        }
                        else
                        {
                            allocateTradeResponse.ResponseStatus = ResponseStatus.Failed;
                            allocateTradeResponse.ResponseMessage = "Asset Type could not be identified.";
                        }
                    }
                }
                else
                {
                    allocateTradeResponse.ResponseStatus = ResponseStatus.Failed;
                    allocateTradeResponse.ResponseMessage = "Security does not exist in Security Master.";
                }

                //Saving Only Allocated Ones
                allocateTradeResponses
                    .Where(In => In.ResponseStatus == ResponseStatus.Success)
                    .ToList()
                    .ForEach(async In =>
                {
                    var result = await _TradeRepository.SaveTrade(((AllocateTradeRequest)In.OriginalRequest).Trade);
                    In.ResponseMessage += result.Message;

                    if (result.Success)
                    {
                        In.ResponseStatus = ResponseStatus.Success;
                    }
                    else
                    {
                        In.ResponseStatus = ResponseStatus.Failed;
                    }
                });

                allocateTradeResponses.Add(allocateTradeResponse);
            }

            return allocateTradeResponses;
        }

        public async Task<string> CheckTradeAllocationStatus(long TradeId)
        {
            API.Trade.Model.Trade Trade = await _TradeRepository.GetTrade(TradeId);
            return Trade.TradeStatus;
        }
        public async Task<List<dynamic>> GetTradeAllocationByDates(List<DateTime> tradeDates)
        {
            List<API.Trade.Model.Trade> Trades = await _TradeRepository.GetTradeAllocationByDates(tradeDates);
            var TradeAllocations = Trades
                .SelectMany(Trade => Trade.TradeAllocations
                .Select(TradeAllocation =>
                new
                {
                    Trade.TradeId,
                    Trade.TradeQuantity,
                    Trade.TradeSide,
                    Trade.TradeStatus,
                    Trade.TradeDate,
                    TradeAllocation.TradeAllocationId,
                    Trade.Security.SecurityIdentifier,
                    TradeAllocation.AccountShotName,
                    TradeAllocation.AllocationQuantity,
                    TradeAllocation.AccountId,

                }
            )).ToList()
                .ConvertAll(In => (dynamic)In);

            return TradeAllocations;
        }

    }
}*/