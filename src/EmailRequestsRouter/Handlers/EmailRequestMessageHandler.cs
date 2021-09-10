using Confluent.Kafka;
using EmailRequestsRouter.Configuration;
using EmailRequestsRouter.Messages;
using EmailRequestsRouter.Processing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailRequestsRouter.Messaging;

namespace EmailRequestsRouter.Handlers
{
    public class EmailRequestMessageHandler : IMessageHandler<EmailRequestMessage>, IDisposable
    {
        private bool _disposed = false;

        private readonly IEnumerable<IProcessingStep<EmailRequestMessage>> _processingSteps;
        private readonly IEmailRequestPublisher _emailRequestPublisher;
        private readonly ILogger<EmailRequestMessageHandler> _logger;

        public EmailRequestMessageHandler(
            // This is a very basic implementation where handler just gets all suitable registered processing steps.
            // For production usage it for sure will require more sophisticated solution with support of features like processing steps ordering,
            // parallel execution of independent steps that do not mutate the message etc.
            IEnumerable<IProcessingStep<EmailRequestMessage>> processingSteps,
            IEmailRequestPublisher emailRequestPublisher,
            ILogger<EmailRequestMessageHandler> logger)
        {
            _processingSteps = processingSteps.ToList();
            _emailRequestPublisher = emailRequestPublisher;
            _logger = logger;
        }

        public async Task Handle(EmailRequestMessage message)
        {
            try
            {
                var processingResults = new List<ProcessingResult>();

                foreach (var step in _processingSteps)
                {
                    processingResults.AddRange(await step.Process(message));
                }

                var processedEmailRequestMessage = new ProcessedEmailRequestMessage(message, processingResults);

                if (processingResults.Any(x => !x.Success))
                {
                    await _emailRequestPublisher.PublishDisqualifiedMessage(processedEmailRequestMessage);
                }
                else
                {
                    await _emailRequestPublisher.PublishValidatedMessage(processedEmailRequestMessage);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Error when processing email request Id:{message.Id} .");
                // This error handling implementation has room for improvement. 
                // Good idea is to have separate error handling blocks for processing sending stages of the flow.
                // Might need to implement retry mechanism.
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _emailRequestPublisher?.Dispose();
            }

            _disposed = true;
        }
    }
}
