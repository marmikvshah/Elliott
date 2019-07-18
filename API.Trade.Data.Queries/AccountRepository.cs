using API.Trade.Common;
using API.Trade.Data.Queries.Interface;
using API.Trade.Shared.Utility;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Trade.Data.Queries
{
    public class AccountRepository : IAccountRepository
    {
        private string _connectionString = string.Empty;
        public AccountRepository(ConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.connectionString;
        }

        public async System.Threading.Tasks.Task<List<Account>> GetAccounts()
        {
            List<Account> Accounts = new List<Account>();
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = await connection.QueryAsync<Account>("SELECT I.* FROM TEST_ACCOUNT");
                Accounts = results.ToList().ConvertAll(x => (Account)x); ;
            }
            return Accounts;
        }

        public async Task<(bool, string)> UpdateAccountNAV(List<Account> accounts)
        {
            string Error = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
                Error += "Error while Saving Trade";
                Error += ex.ToString();
                if (ex.InnerException != null)
                    Error += ex.InnerException.ToString();
            }
            return (true, Error);
        }
    }
}
