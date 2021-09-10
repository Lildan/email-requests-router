using Confluent.Kafka;
using EmailRequestsRouter.Configuration;
using EmailRequestsRouter.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EmailRequestsRouter.Messaging
{
    public class KafkaEmailRequestPublisher : IEmailRequestPublisher
    {
        private bool _disposed = false;

        private readonly IProducer<string, string> _kafkaProducer;
        private readonly KafkaProducerConfiguration _producerConfiguration;
        private readonly ILogger<KafkaEmailRequestPublisher> _logger;

        public KafkaEmailRequestPublisher(
            IProducer<string, string> kafkaProducer,
            KafkaProducerConfiguration producerConfiguration,
            ILogger<KafkaEmailRequestPublisher> logger)
        {
            _kafkaProducer = kafkaProducer;
            _producerConfiguration = producerConfiguration;
            _logger = logger;
        }

        public async Task PublishValidatedMessage(ProcessedEmailRequestMessage message)
        {
            await PublishToTopic(_producerConfiguration.ValidatedTopicName, message);
        }

        public async Task PublishDisqualifiedMessage(ProcessedEmailRequestMessage message)
        {
            await PublishToTopic(_producerConfiguration.DisqualifiedTopicName, message);
        }

        private async Task PublishToTopic(string topicName, ProcessedEmailRequestMessage message)
        {
            try
            {
                var kafkaMessage = new Message<string, string>
                {
                    Key = message.Id.ToString(),
                    Value = JsonConvert.SerializeObject(message)
                };
                await _kafkaProducer.ProduceAsync(topicName, kafkaMessage);
            }
            catch (JsonException e)
            {
                _logger.LogError(e, $"Failed to serialize message. Id {message.Id} ");
            }
            catch (KafkaException e)
            {
                _logger.LogError(e, $"Message publishing failed. Id {message.Id} ");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown error when publishing message. Id {message.Id} ");
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
                _kafkaProducer?.Flush();
                _kafkaProducer?.Dispose();
            }

            _disposed = true;
        }
    }
}
