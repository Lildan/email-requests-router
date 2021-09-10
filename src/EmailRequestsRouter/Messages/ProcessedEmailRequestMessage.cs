using System;
using System.Collections.Generic;
using EmailRequestsRouter.Domain;
using EmailRequestsRouter.Processing;

namespace EmailRequestsRouter.Messages
{
    public class ProcessedEmailRequestMessage
    {
        public ProcessedEmailRequestMessage(EmailRequestMessage message, IEnumerable<ProcessingResult> results)
        {
            Id = message.Id;
            Client = message.Client;
            EmailPayload = message.EmailPayload;
            SubmittedAt = message.SubmittedAt;
            ProcessingResults = results;
        }

        public Guid Id { get; set; }
        public Client Client { get; set; }
        public EmailMessage EmailPayload { get; set; }
        public DateTime SubmittedAt { get; set; }
        public IEnumerable<ProcessingResult> ProcessingResults { get; set; }
    }
}
