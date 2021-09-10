using Microsoft.Extensions.Configuration;

namespace EmailRequestsRouter.Configuration
{
    public class KafkaConsumerConfiguration
    {
        public string BootstrapServer { get; set; }
        public string TopicName { get; set; }
        public string ConsumerGroup { get; set; }

        public KafkaConsumerConfiguration(IConfiguration config)
        {
            var section = config.GetSection("KAFKACONSUMER");

            BootstrapServer = section["BOOTSTRAPSERVER"];
            TopicName = section["TOPICNAME"];
            ConsumerGroup = section["CONSUMERGROUP"];
        }
    }
}
