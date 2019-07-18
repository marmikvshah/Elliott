using API.Trade.Domain.Commands.Allocate.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Allocate.Validators
{
    public class AllocateTradeCommandRequestsValidator : AbstractValidator<AllocateTradeCommandRequests>
    {
        public AllocateTradeCommandRequestsValidator()
        {
            RuleForEach(x => x.Requests).SetValidator(new AllocateTradeCommandRequestValidator());
        }
    }
}
