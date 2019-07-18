using API.Trade.Domain.Commands.Allocate;
using FluentValidation;

namespace API.Trade.Domain.Queries.Trade.Validators
{

    public class GetTradeStatusQueryRequestValidator : AbstractValidator<GetTradeStatusQueryRequest>
    {

        public GetTradeStatusQueryRequestValidator()
        {
            RuleFor(CreateTradeRequest => CreateTradeRequest.Trade)
                .Must(ValidateQuantity)
                .WithMessage("Invalid Trade Id.");
        }

        private bool ValidateQuantity(Common.Trade value)
        {
            return value != null && !string.IsNullOrEmpty(value.TradeId.ToString()) && value.TradeId != -1;
        }
    }
}
