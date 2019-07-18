using API.Trade.Data.Commands.Repositories.Interfaces;
using API.Trade.Shared;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using API.Trade.Common;
using System.Threading.Tasks;
using API.Trade.Shared.Utility;
using System.Data;

namespace API.Trade.Data.Commands
{
    public class TradeCreateRepository : ITradeCreateRepository
    {
        private readonly string _connectionString;
        public TradeCreateRepository(ConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.connectionString;
        }

        public async Task<(bool Success, string Message, API.Trade.Common.Trade Trade)> CreateTrade(API.Trade.Common.Trade Trade)
        {
            string Error = string.Empty;
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_TRADE_ID", Trade.TradeId, OracleDbType.Int64, ParameterDirection.Input);
                dyParam.Add("P_TRADE_QUANTITY", Trade.TradeQuantity, OracleDbType.Double, ParameterDirection.Input);
                dyParam.Add("P_TRADE_DATE", Trade.TradeDate, OracleDbType.Date, ParameterDirection.Input);
                dyParam.Add("P_TRADE_SIDE", Trade.TradeSide, OracleDbType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_TRADE_STATUS", Trade.TradeStatus, OracleDbType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_TRADE_PRICE", Trade.Price, OracleDbType.Double, ParameterDirection.Input);
                dyParam.Add("P_INSTRUMENT_ID", Trade.InstrumentId, OracleDbType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_TRADE_PRICE", Trade.Price, OracleDbType.Double, ParameterDirection.Input);

                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await SqlMapper.ExecuteAsync(connection, "PACKAGE.CREATE_TRADE", param: dyParam, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Error += "Error while Creating Trade";
                Error += ex.ToString();
                if (ex.InnerException != null)
                    Error += ex.InnerException.ToString();
            }
            return (true, Error, Trade);
        }
    }
}
