using System;
using EmailRequestsRouter.Messages;
using System.Threading.Tasks;

namespace EmailRequestsRouter.Messaging
{
    /// <summary>
    /// This interface is an abstraction to component that is responsible for communication with specific messaging system.
    /// This interface can have multiple implementations e.g. for RabbitMQ, Amazon SQS, Azure Queue storage etc.
    /// </summary>
    public interface IEmailRequestPublisher: IDisposable
    {
        public Task PublishValidatedMessage(ProcessedEmailRequestMessage message);
        public Task PublishDisqualifiedMessage(ProcessedEmailRequestMessage message);
    }
}
