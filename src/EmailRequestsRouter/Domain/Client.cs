using System;

namespace EmailRequestsRouter.Domain
{
    public class Client
    {
        public Guid Id { get; set; }

        // Might be extended to include more client data that will be used during email routing
    }
}
