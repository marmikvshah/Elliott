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
    public class SecurityRepository : ISecurityRepository
    {

        private string _connectionString = string.Empty;
        public SecurityRepository(ConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.connectionString;
        }

        public async Task<Security> GetSecurity(string SecurityIdentifier)
        {
            Security Security = new Security();
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = await connection.QueryAsync<Security>(string.Format(@"
                            SELECT I.* FROM TEST_INSTRUMENT I WHERE I.SECURITY_INDENTIFIER = '{0}'", SecurityIdentifier));
                Security = results.FirstOrDefault<Security>();
            }
            return Security;
        }
    }
}
