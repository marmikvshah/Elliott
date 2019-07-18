using API.Trade.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Domain.Commands.Create.Validators
{
    public class CreateTradeCommandRequestValidator : AbstractValidator<CreateTradeCommandRequest>
    {
        public CreateTradeCommandRequestValidator()
        {
            RuleFor(CreateTradeRequest => CreateTradeRequest.Trade.Price)
                .Must(ValidateQuantity)
                .WithMessage("Invalid Price.");

            RuleFor(CreateTradeRequest => CreateTradeRequest.Trade.TradeQuantity)
                .Must(ValidateQuantity)
                .WithMessage("Invalid Quantity.");

            RuleFor(CreateTradeRequest => CreateTradeRequest.Trade.Security)
                .Must(ValidateSecurity)
                .WithMessage("Invalid Security.");

            RuleFor(CreateTradeRequest => CreateTradeRequest.Trade)
                .Must(ValidateTrade)
                .WithMessage("Trade already exist.");
        }

        private bool ValidateQuantity(double value)
        {
            return value != 0;
        }

        private bool ValidateSecurity(Security value)
        {
            return value != null
                && !string.IsNullOrEmpty(value.SecurityIdentifier);
        }

        private bool ValidateTrade(API.Trade.Common.Trade value)
        {
            return value != null;
        }
    }
}
