using Confluent.Kafka;
using EmailRequestsRouter.Configuration;
using EmailRequestsRouter.Handlers;
using EmailRequestsRouter.Messages;
using EmailRequestsRouter.Messaging;
using EmailRequestsRouter.Processing;
using EmailRequestsRouter.Processing.MessageProcessors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using EmailRequestsRouter.Processing.Processors;

namespace EmailRequestsRouter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new Random()); // For dummy business logic
                    services.AddSingleton<KafkaConsumerConfiguration>();
                    services.AddSingleton<KafkaProducerConfiguration>();

                    services.AddTransient(CreateKafkaConsumer<string, string>);
                    // Kafka producer is thread safe so it can be registered as singleton and used by multiple threads to foster batching.
                    services.AddSingleton(CreateKafkaProducer<string, string>);
                    services.AddTransient<IEmailRequestPublisher, KafkaEmailRequestPublisher>();

                    services.AddTransient<IProcessingStep<EmailRequestMessage>, SourceEmailFilter>(); 
                    services.AddTransient<IProcessingStep<EmailRequestMessage>, UnsubscribedReceiversFilter>(); 

                    services.AddTransient<IMessageHandler<EmailRequestMessage>, EmailRequestMessageHandler>();
                    services.AddHostedService<Worker>();
                });

        private static IConsumer<TKey,TValue> CreateKafkaConsumer<TKey,TValue>(IServiceProvider sp) where TValue: class
        {
            var kafkaConsumerConfig = sp.GetRequiredService<KafkaConsumerConfiguration>();

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaConsumerConfig.BootstrapServer,
                GroupId = kafkaConsumerConfig.ConsumerGroup,
                AllowAutoCreateTopics = true,
            };
            var consumer = new ConsumerBuilder<TKey, TValue>(consumerConfig).Build();
            consumer.Subscribe(kafkaConsumerConfig.TopicName);
            return consumer;
        }

        private static IProducer<TKey,TValue> CreateKafkaProducer<TKey,TValue>(IServiceProvider sp) where TValue : class
        {
            var kafkaProducerConfig = sp.GetRequiredService<KafkaProducerConfiguration>();
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaProducerConfig.BootstrapServer,
                // Enabling idempotent delivery for producer which will automatically generate 
                EnableIdempotence = true
            };

            return new ProducerBuilder<TKey, TValue>(producerConfig).Build();
        }
    }
}
