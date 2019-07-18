using API.Trade.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Trade.Data.Queries.Interface
{
    public interface IAccountRepository
    {
        System.Threading.Tasks.Task<List<Account>> GetAccounts();
        Task<(bool, string)> UpdateAccountNAV(List<Account> accounts);
    }
}
