using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Shared.Messaging
{
    public interface ITradeRequest<T> : IRequest<T>
    {

    }
}
