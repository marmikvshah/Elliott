using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Create.Validators
{
    public class CreateTradeCommandRequestsValidator : AbstractValidator<CreateTradeCommandRequests>
    {
        public CreateTradeCommandRequestsValidator()
        {
            RuleForEach(x => x.Requests).SetValidator(new CreateTradeCommandRequestValidator());
        }
    }
}
