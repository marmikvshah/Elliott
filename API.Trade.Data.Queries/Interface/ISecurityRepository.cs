using API.Trade.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Trade.Data.Queries.Interface
{
    public interface ISecurityRepository
    {
        Task<Security> GetSecurity(string SecurityIdentifier);
    }
}
