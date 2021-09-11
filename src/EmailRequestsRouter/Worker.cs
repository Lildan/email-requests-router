using Confluent.Kafka;
using EmailRequestsRouter.Handlers;
using EmailRequestsRouter.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EmailRequestsRouter
{
    public class Worker : BackgroundService
    {
        private bool _disposed = false;

        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<string, string> _kafkaConsumer;
        private readonly IMessageHandler<EmailRequestMessage> _messageHandler;

        public Worker(
            ILogger<Worker> logger, 
            IConsumer<string, string> kafkaConsumer,
            IMessageHandler<EmailRequestMessage> messageHandler)
        {
            _logger = logger;
            _kafkaConsumer = kafkaConsumer;
            _messageHandler = messageHandler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(async () => await StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }
        
        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _kafkaConsumer.Consume(cancellationToken);
                    EmailRequestMessage emailRequestMessage;
                    try                    
                    {
                        emailRequestMessage = JsonConvert.DeserializeObject<EmailRequestMessage>(consumeResult.Message.Value);
                    }
                    catch (JsonException e)
                    {
                        _logger.LogError(e, $"Failed to deserialize message.");
                        continue;
                    }
                    await _messageHandler.Handle(emailRequestMessage);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    _logger.LogError(e,$"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }

                catch (Exception e)
                {
                    _logger.LogError($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
                _kafkaConsumer.Dispose();
                _messageHandler.Dispose();
            }

            _disposed = true;
            base.Dispose();
        }
    }
}
