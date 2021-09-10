using Microsoft.Extensions.Configuration;

namespace EmailRequestsRouter.Configuration
{
    public class KafkaProducerConfiguration
    {
        public string BootstrapServer { get; set; }
        public string ValidatedTopicName { get; set; }
        public string DisqualifiedTopicName { get; set; }

        public KafkaProducerConfiguration(IConfiguration config)
        {
            var section = config.GetSection("KAFKAPRODUCER");

            BootstrapServer = section["BOOTSTRAPSERVER"];
            ValidatedTopicName = section["VALIDATEDTOPICNAME"];
            DisqualifiedTopicName = section["DISQUALIFIEDTOPICNAME"];
        }
    }
}
