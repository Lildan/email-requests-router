using EmailRequestsRouter.Handlers;
using EmailRequestsRouter.Messages;
using EmailRequestsRouter.Messaging;
using EmailRequestsRouter.Processing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EmailRequestsRouter.Tests
{
    public class EmailRequestMessageHandlerTests
    {
        private readonly Mock<IEmailRequestPublisher> _messagePublisher;

        public EmailRequestMessageHandlerTests()
        {
            _messagePublisher = new Mock<IEmailRequestPublisher>();
        }

        [Fact]
        public async Task Handle_ShouldUseAllRegisteredProcessingSteps()
        {
            var messagePublisher = new Mock<IEmailRequestPublisher>();
            var logger = new Mock<ILogger<EmailRequestMessageHandler>>();
            var messageProcessor1 = new Mock<IProcessingStep<EmailRequestMessage>>();
            messageProcessor1
                .Setup(x => x.Process(It.IsAny<EmailRequestMessage>()))
                .ReturnsAsync(new[]{ProcessingResult.Successful("")});

            var messageProcessor2 = new Mock<IProcessingStep<EmailRequestMessage>>();
            messageProcessor2
                .Setup(x => x.Process(It.IsAny<EmailRequestMessage>()))
                .ReturnsAsync(new[] { ProcessingResult.Successful("") });

            var message = new EmailRequestMessage();
            var handler = new EmailRequestMessageHandler(
                new []{messageProcessor1.Object, messageProcessor2.Object},
                messagePublisher.Object,
                logger.Object);

            await handler.Handle(message);

            messageProcessor1.Verify(x=>x.Process(message), Times.Once);
            messageProcessor2.Verify(x => x.Process(message), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProcessingFailed_ShouldPublishToDisqualifiedCategory()
        {
            var messagePublisher = new Mock<IEmailRequestPublisher>();
            var logger = new Mock<ILogger<EmailRequestMessageHandler>>();
            var messageProcessor1 = new Mock<IProcessingStep<EmailRequestMessage>>();
            messageProcessor1
                .Setup(x => x.Process(It.IsAny<EmailRequestMessage>()))
                .ReturnsAsync(new[] { ProcessingResult.Failed("") });

            var messageProcessor2 = new Mock<IProcessingStep<EmailRequestMessage>>();
            messageProcessor2
                .Setup(x => x.Process(It.IsAny<EmailRequestMessage>()))
                .ReturnsAsync(new[] { ProcessingResult.Successful("") });

            var message = new EmailRequestMessage();
            var handler = new EmailRequestMessageHandler(
                new[] { messageProcessor1.Object, messageProcessor2.Object },
                messagePublisher.Object,
                logger.Object);

            await handler.Handle(message);

            messagePublisher.Verify(x => x.PublishDisqualifiedMessage(It.IsAny<ProcessedEmailRequestMessage>()), Times.Once());
            messagePublisher.Verify(x => x.PublishValidatedMessage(It.IsAny<ProcessedEmailRequestMessage>()), Times.Never());
        }

        [Fact]
        public async Task Handle_WhenProcessingSuccessful_ShouldPublishToValidatedCategory()
        {
            var messagePublisher = new Mock<IEmailRequestPublisher>();
            var logger = new Mock<ILogger<EmailRequestMessageHandler>>();
            var messageProcessor1 = new Mock<IProcessingStep<EmailRequestMessage>>();
            messageProcessor1
                .Setup(x => x.Process(It.IsAny<EmailRequestMessage>()))
                .ReturnsAsync(new[] { ProcessingResult.Successful("") });

            var messageProcessor2 = new Mock<IProcessingStep<EmailRequestMessage>>();
            messageProcessor2
                .Setup(x => x.Process(It.IsAny<EmailRequestMessage>()))
                .ReturnsAsync(new[] { ProcessingResult.Successful("") });

            var message = new EmailRequestMessage();
            var handler = new EmailRequestMessageHandler(
                new[] { messageProcessor1.Object, messageProcessor2.Object },
                messagePublisher.Object,
                logger.Object);

            await handler.Handle(message);

            messagePublisher.Verify(x=>x.PublishDisqualifiedMessage(It.IsAny<ProcessedEmailRequestMessage>()), Times.Never());
            messagePublisher.Verify(x=>x.PublishValidatedMessage(It.IsAny<ProcessedEmailRequestMessage>()), Times.Once());
        }
    }
}
