using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.Trade.Shared.Messaging
{
    public class TradeMessaging : ITradeMessaging
    {
        private readonly IMediator _mediator;
        public TradeMessaging(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}
