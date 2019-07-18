using API.Trade.Common;
using API.Trade.Data.Commands.Repositories.Interfaces;
using API.Trade.Shared;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API.Trade.Data.Commands
{
    public class TradeAllocateRepository : ITradeAllocateRepository
    {

        private readonly string _connectionString;

        public TradeAllocateRepository(ConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.connectionString;
        }

        public async Task<(bool Success, string Error, Common.Trade Trade)> AllocateTrade(Common.Trade Trade)
        {
            string Error = string.Empty;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleTransaction OracleTransaction = connection.BeginTransaction();

                    var tradeParam = new OracleDynamicParameters();

                    tradeParam.Add("P_TRADE_ID", Trade.TradeId, OracleDbType.Int64, ParameterDirection.Input);
                    tradeParam.Add("P_TRADE_QUANTITY", Trade.TradeQuantity, OracleDbType.Double, ParameterDirection.Input);
                    tradeParam.Add("P_TRADE_DATE", Trade.TradeDate, OracleDbType.Date, ParameterDirection.Input);
                    tradeParam.Add("P_TRADE_SIDE", Trade.TradeSide, OracleDbType.Varchar2, ParameterDirection.Input);
                    tradeParam.Add("P_TRADE_STATUS", Trade.TradeStatus, OracleDbType.Varchar2, ParameterDirection.Input);
                    tradeParam.Add("P_TRADE_PRICE", Trade.Price, OracleDbType.Double, ParameterDirection.Input);
                    tradeParam.Add("P_INSTRUMENT_ID", Trade.InstrumentId, OracleDbType.Varchar2, ParameterDirection.Input);

                    await SqlMapper.ExecuteAsync(connection, "PACKAGE.ALLOCATE_TRADE", param: tradeParam, commandType: CommandType.StoredProcedure);

                    foreach(Common.TradeAllocation TradeAllocation in Trade.TradeAllocations.Where(In => In.AllocationQuantity > 0).ToList())
                    {
                        var tradeAllocationParam = new OracleDynamicParameters();

                        tradeAllocationParam.Add("P_TRADE_ID", TradeAllocation.TradeId, OracleDbType.Int64, ParameterDirection.Input);
                        tradeAllocationParam.Add("P_TRADE_ALLOCATION_QUANTITY", TradeAllocation.AllocationQuantity, OracleDbType.Int64, ParameterDirection.Input);
                        tradeAllocationParam.Add("P_ACCOuNT_ID", TradeAllocation.AccountId, OracleDbType.Int64, ParameterDirection.Input);

                        await SqlMapper.ExecuteAsync(connection, "PACKAGE.ALLOCATE_TRADE_ALLOCATION", param: tradeAllocationParam, commandType: CommandType.StoredProcedure);
                    }

                    OracleTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Error += "Error while allocating Trade";
                Error += ex.ToString();
                if (ex.InnerException != null)
                    Error += ex.InnerException.ToString();
                return (false, Error, Trade);
            }
            return (true, Error, Trade);
        }
    }
}
