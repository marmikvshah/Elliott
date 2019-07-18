using System;
using System.Collections.Generic;

namespace API.Trade.Data.Queries.Interface
{
    public interface ITradeGetRepository
    {

        System.Threading.Tasks.Task<List<API.Trade.Common.Trade>> GetTradeAllocationByDates(List<DateTime> TradeDates);
        System.Threading.Tasks.Task<API.Trade.Common.Trade> GetTrade(long TradeId);
        System.Threading.Tasks.Task<List<API.Trade.Common.TradeAllocation>> GetTradeAllocation(long TradeId);

        System.Threading.Tasks.Task<List<API.Trade.Common.Trade>> GetAllTradesBySecyrityIdByDate(string SecurityIdentifier, DateTime TradeDate);
    }
}
