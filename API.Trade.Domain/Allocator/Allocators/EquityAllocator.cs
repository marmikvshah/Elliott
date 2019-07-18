using System;
using System.Collections.Generic;
using System.Text;
using API.Trade.Common;
using System.Linq;
using API.Trade.Common.ViewModels;

namespace API.Trade.Domain
{
    internal class EquityAllocator : IEquityAllocator
    {
        public (Common.Trade Trade, string AllocatorMessage) AllocateEquity(Common.Trade trade, List<Common.Trade> existingAllocatedTrades)
        {

            List<Account> UpdatedAccounts = new List<Account>();

            StringBuilder StringBuilder = new StringBuilder();

            StringBuilder.AppendLine("Before Allocations.");
            StringBuilder.AppendLine(trade.ToString());

            TradeAllocation TradeAllocationA = trade.TradeAllocations.Find(In => In.AccountShotName == "A");
            TradeAllocation TradeAllocationB = trade.TradeAllocations.Find(In => In.AccountShotName == "B");

            //Assumption : I am taking absolute market value of all the Trades; not considering Trade Side; that is what is my interpreation from document ("If the trade value of the day's equity allocations")

            if (trade.TradeSide == "Sell")
            {
                double TotalMarketValue = existingAllocatedTrades
                 .SelectMany(Trade => Trade.TradeAllocations
                 .Select(TradeAllocation =>
                 new TradeAllocationViewModel
                 {
                     TradeSide = Trade.TradeSide,
                     TradePrice = Trade.Price,
                     AccountShotName = TradeAllocation.AccountShotName,
                     AllocationQuantity = TradeAllocation.AllocationQuantity,
                     AccountId = TradeAllocation.AccountId,
                 }))
                 .ToList()
                 .Sum(In => In.AllocationQuantity * In.TradePrice);


                double AMarketValue = existingAllocatedTrades
                 .SelectMany(Trade => Trade.TradeAllocations.Where(In => In.AccountShotName == "A").ToList()
                 .Select(TradeAllocation =>
                 new TradeAllocationViewModel
                 {
                     TradeSide = Trade.TradeSide,
                     TradePrice = Trade.Price,
                     AccountShotName = TradeAllocation.AccountShotName,
                     AllocationQuantity = TradeAllocation.AllocationQuantity,
                     AccountId = TradeAllocation.AccountId,
                 }))
                 .ToList()
                 .Sum(In => In.AllocationQuantity * In.TradePrice);

                double BMarketValue = TotalMarketValue - AMarketValue;
                double AMarketValuePct = AMarketValue / TotalMarketValue;
                double BMarketValuePct = 1 - AMarketValuePct;

                bool AFundThresholdReached = false;

                if (Math.Abs(AMarketValue - 20000000) < 0.1)
                {
                    AFundThresholdReached = true;
                }

                if (AFundThresholdReached)
                {
                    double NewBMarketValuePct = BMarketValue + (trade.Price * trade.TradeQuantity) / TotalMarketValue + (trade.Price * trade.TradeQuantity);
                    if (NewBMarketValuePct < 0.8)
                    {
                        TradeAllocationB.AllocationQuantity = trade.TradeQuantity;
                    }
                    else
                    {
                        double RemainingPct = NewBMarketValuePct - 0.8;
                        double RemainingValue =  (TotalMarketValue + (trade.Price * trade.TradeQuantity) * RemainingPct);
                        TradeAllocationB.AllocationQuantity = Math.Floor(RemainingValue / trade.Price);
                        TradeAllocationA.AllocationQuantity = trade.TradeQuantity - TradeAllocationB.AllocationQuantity;
                    }
                }
                else
                {
                    double FundATodaysAllocation = AMarketValue;
                    if (FundATodaysAllocation >= 20000000)
                    {
                        TradeAllocationB.AllocationQuantity = trade.TradeQuantity;
                    }
                    else if (FundATodaysAllocation < 20000000)
                    {
                        double RemainingAMarketValue = 20000000 - FundATodaysAllocation;

                        double ReadyToAllocateQuantity = 0;
                        if (RemainingAMarketValue >= (trade.TradeQuantity * trade.Price) * 0.20) //Regular when Summation of Buy and Sell has not reached 20m
                        {
                            ReadyToAllocateQuantity = Math.Floor(((trade.TradeQuantity * trade.Price) * 0.20) / trade.Price);
                        }
                        else
                        {
                            ReadyToAllocateQuantity = Math.Floor(RemainingAMarketValue / trade.Price);
                        }
                        TradeAllocationA.AllocationQuantity = ReadyToAllocateQuantity;
                        TradeAllocationB.AllocationQuantity = trade.TradeQuantity - TradeAllocationA.AllocationQuantity;
                    }
                }
            }
            else if (trade.TradeSide == "Buy")
            {

                var ATradeAllocations = existingAllocatedTrades
                 .SelectMany(Trade => Trade.TradeAllocations.Where(In => In.AccountShotName == "A").ToList()
                 .Select(TradeAllocation =>
                 new TradeAllocationViewModel
                 {
                     TradeSide = Trade.TradeSide,
                     TradePrice = Trade.Price,
                     AccountShotName = TradeAllocation.AccountShotName,
                     AllocationQuantity = TradeAllocation.AllocationQuantity,
                     AccountId = TradeAllocation.AccountId,
                 })).ToList();

                double FundATodaysAllocation = ATradeAllocations.Sum(In => In.AllocationQuantity * In.TradePrice);
                if (FundATodaysAllocation >= 20000000)
                {
                    TradeAllocationB.AllocationQuantity = trade.TradeQuantity;
                }
                else if (FundATodaysAllocation < 20000000)
                {
                    double RemainingAMarketValue = 20000000 - FundATodaysAllocation;

                    double ReadyToAllocateQuantity = 0;
                    if (RemainingAMarketValue >= (trade.TradeQuantity * trade.Price) * 0.20) //Regular when Summation of Buy and Sell has not reached 20m
                    {
                        ReadyToAllocateQuantity = Math.Floor(((trade.TradeQuantity * trade.Price) * 0.20) / trade.Price);
                    }
                    else
                    {
                        ReadyToAllocateQuantity = Math.Floor(RemainingAMarketValue / trade.Price);
                    }
                    TradeAllocationA.AllocationQuantity = ReadyToAllocateQuantity;
                    TradeAllocationB.AllocationQuantity = trade.TradeQuantity - TradeAllocationA.AllocationQuantity;
                }
            }

            StringBuilder.AppendLine("After Allocations.");
            StringBuilder.AppendLine(trade.ToString());

            return (trade, StringBuilder.ToString());
        }
    }
}
