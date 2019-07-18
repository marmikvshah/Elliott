using API.Trade.Domain.Commands.Allocate;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.Trade.Domain.Queries.Trade.Validators
{
    public class GetTradeAllocationByDayQueryRequestValidator : AbstractValidator<GetTradeAllocationByDayQueryRequest>
    {

        public GetTradeAllocationByDayQueryRequestValidator()
        {
            RuleFor(CreateTradeRequest => CreateTradeRequest.RequestedDates)
                .Must(ValidateQuantity)
                .WithMessage("Invalid Dates.");
        }

        private bool ValidateQuantity(List<DateTime> value)
        {
            return value != null && value.Count() > 0;
        }
    }
}
