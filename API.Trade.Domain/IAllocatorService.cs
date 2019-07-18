//using System;
//using System.Collections.Generic;
//using System.Text;
//using API.Trade.Model;
//using System.Threading.Tasks;

//namespace API.Trade.Domain
//{
//    public interface IAllocatorService
//    {
//        Task<List<CreateTradeResponse>> CreateTrades(List<CreateTradeRequest> createTradeRequest);
//        Task<List<AllocateTradeResponse>> AllocateTrades(List<AllocateTradeRequest> allocateTradeRequest);
//        Task<string> CheckTradeAllocationStatus(long TradeId);
//        Task<List<dynamic>> GetTradeAllocationByDates(List<DateTime> TradeDates);
//    }
//}
