using API.Trade.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Trade.Data.Commands.Repositories.Interfaces
{
    public interface ITradeAllocateRepository
    {
        Task<(bool Success, string Error, API.Trade.Common.Trade Trade)> AllocateTrade(API.Trade.Common.Trade Trade);
    }
}
