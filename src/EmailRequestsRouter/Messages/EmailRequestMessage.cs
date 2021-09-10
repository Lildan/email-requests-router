using System;
using EmailRequestsRouter.Domain;

namespace EmailRequestsRouter.Messages
{
    public class EmailRequestMessage
    {
        public Guid Id { get; set; }
        public Client Client { get; set; }
        public EmailMessage EmailPayload { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
