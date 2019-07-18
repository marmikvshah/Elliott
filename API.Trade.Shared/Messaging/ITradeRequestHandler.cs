using MediatR;

namespace API.Trade.Shared
{
    public interface ITradeRequestHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
    }
}
