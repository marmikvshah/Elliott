using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Trade.Data.Commands.Repositories.Interfaces
{
    public interface ITradeCreateRepository
    {
        Task<(bool Success, string Message, API.Trade.Common.Trade Trade)> CreateTrade(API.Trade.Common.Trade Trade);
    }
}
