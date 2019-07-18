using API.Trade.Common;
using API.Trade.Data.Queries.Interface;
using API.Trade.Shared.Utility;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Trade.Data.Queries
{
    public class TradeGetRepository : ITradeGetRepository
    {


        private string _connectionString = string.Empty;

        public TradeGetRepository(ConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.connectionString;
        }

        public async System.Threading.Tasks.Task<List<API.Trade.Common.Trade>> GetTradeAllocationByDates(List<DateTime> TradeDates)
        {
            List<API.Trade.Common.Trade> Trades = new List<API.Trade.Common.Trade>();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = connection.QueryMultiple(
                    string.Format(
                    @"      SELECT * FROM  TES_TRADE WHERE TRADE_DATE IN ('{0}');
                            SELECT I.*  FROM TEST_INSTRUMENT I, TES_TRADE T WHERE T.INSTRUMENT_ID = I.INSTRUMENT_ID AND T.TRADE_DATE IN ('{0}');
                            SELECT TA.* FROM TEST_TRADE T,  TEST_TRADE_ALLOCATION TA WHERE T.TRADE_ID = TA.TRADE_ID AND T.TRADE_DATE IN  ('{0}')",
                                string.Join(", ", TradeDates.Select(In => In.ToString("dd-MMM-yyyy")).ToArray())));

                var TradeMessage = results.Read<API.Trade.Common.Trade>();
                var Securities = results.Read<API.Trade.Common.Security>();
                var TradeAllocations = results.Read<API.Trade.Common.TradeAllocation>();

                Trades = TradeMessage
                        .ToList()
                        .ConvertAll(In => (API.Trade.Common.Trade)In)
                        .ToList();

                Trades.ForEach(In =>
                {
                    In
                    .TradeAllocations
                    .AddRange(TradeAllocations.Where(TradAlloc => TradAlloc.TradeId == In.TradeId)
                    .ToList());
                });

                Securities
                        .ToList()
                        .ConvertAll(In => (API.Trade.Common.Security)In)
                        .ToList()
                        .ForEach(In =>
                        {
                            Trades
                            .Where(InTrade => InTrade.InstrumentId == In.InstrumentId)
                            .Select(InTrade => InTrade.Security = In.DeepClone<Security>());
                        });
            }
            return Trades;
        }

        public async System.Threading.Tasks.Task<API.Trade.Common.Trade> GetTrade(long TradeId)
        {
            API.Trade.Common.Trade Trade = new Trade.Common.Trade();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = connection.QueryMultiple(@"
                            SELECT * FROM  TES_TRADE WHERE TRADE_ID = {0};
                            SELECT I.* FROM TEST_INSTRUMENT I, TES_TRADE T WHERE T.INSTRUMENT_ID = I.INSTRUMENT_ID AND T.TRADE_ID = {0};
                            SELECT TA.* FROM TEST_TRADE T,  TEST_TRADE_ALLOCATION TA WHERE T.TRADE_ID = TA.TRADE_ID AND T.TRADE_ID = {0}
                        ");

                var TradeMessage = results.Read<API.Trade.Common.Trade>();
                var Security = results.Read<API.Trade.Common.Security>();
                var TradeAllocations = results.Read<API.Trade.Common.TradeAllocation>();

                Trade = TradeMessage.FirstOrDefault();
                Trade.Security = Security.FirstOrDefault();
                Trade.TradeAllocations.AddRange(TradeAllocations);
            }
            return Trade;
        }

        public async System.Threading.Tasks.Task<List<API.Trade.Common.TradeAllocation>> GetTradeAllocation(long TradeId)
        {
            API.Trade.Common.Trade Trade = new Trade.Common.Trade();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = connection.QueryMultiple(@"
                            SELECT * FROM  TES_TRADE WHERE TRADE_ID = {0};
                            SELECT I.* FROM TEST_INSTRUMENT I, TES_TRADE T WHERE T.INSTRUMENT_ID = I.INSTRUMENT_ID AND T.TRADE_ID = {0};
                            SELECT TA.* FROM TEST_TRADE T,  TEST_TRADE_ALLOCATION TA WHERE T.TRADE_ID = TA.TRADE_ID AND T.TRADE_ID = {0}
                        ");

                var TradeMessage = results.Read<API.Trade.Common.Trade>();
                var Security = results.Read<API.Trade.Common.Security>();
                var TradeAllocations = results.Read<API.Trade.Common.TradeAllocation>();

                Trade = TradeMessage.FirstOrDefault();
                Trade.Security = Security.FirstOrDefault();
                Trade.TradeAllocations.AddRange(TradeAllocations);
            }
            return Trade.TradeAllocations;
        }

        public async System.Threading.Tasks.Task<List<API.Trade.Common.Trade>> GetAllTradesBySecyrityIdByDate(string SecurityIdentifier, DateTime TradeDate)
        {
            List<API.Trade.Common.Trade> Trades = new List<Common.Trade>();


            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = connection.QueryMultiple(
                    string.Format(
                    @"      SELECT T.*  FROM TEST_INSTRUMENT I, TES_TRADE T WHERE T.INSTRUMENT_ID = I.INSTRUMENT_ID AND T.TRADE_DATE IN ('{0}') AND I.SECURITY_IDENTIFIER = '{1}' AND I.TRADE_STATUS = 'ALLOCATED';
                            SELECT I.*  FROM TEST_INSTRUMENT I.SECURITY_IDENTIFIER = '{1}';
                            SELECT TA.* FROM TEST_TRADE T,  TEST_TRADE_ALLOCATION TA, TEST_INSTRUMENT I WHERE T.TRADE_ID = TA.TRADE_ID AND T.INSTRUMENT_ID = I.INSTRUMENT_ID AND T.TRADE_DATE IN ('{0}') AND I.SECURITY_IDENTIFIER = '{1}'",
                                TradeDate.ToString("dd-MMM-yyyy"), SecurityIdentifier));

                var TradeMessage = results.Read<API.Trade.Common.Trade>();
                var Securities = results.Read<API.Trade.Common.Security>();
                var TradeAllocations = results.Read<API.Trade.Common.TradeAllocation>();

                Trades = TradeMessage
                        .ToList()
                        .ConvertAll(In => (API.Trade.Common.Trade)In)
                        .ToList();

                Trades.ForEach(In =>
                {
                    In
                    .TradeAllocations
                    .AddRange(TradeAllocations.Where(TradAlloc => TradAlloc.TradeId == In.TradeId)
                    .ToList());
                });

                Securities
                        .ToList()
                        .ConvertAll(In => (API.Trade.Common.Security)In)
                        .ToList()
                        .ForEach(In =>
                        {
                            Trades
                            .Where(InTrade => InTrade.InstrumentId == In.InstrumentId)
                            .Select(InTrade => InTrade.Security = In.DeepClone<Security>());
                        });
            }

            return Trades;
        }
    }
}
