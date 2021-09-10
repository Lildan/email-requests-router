using System;
using System.Threading.Tasks;

namespace EmailRequestsRouter.Handlers
{
    public interface IMessageHandler<T>: IDisposable
    {
        Task Handle(T message);
    }
}
