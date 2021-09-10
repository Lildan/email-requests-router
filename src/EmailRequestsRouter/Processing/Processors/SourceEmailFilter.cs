using EmailRequestsRouter.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailRequestsRouter.Processing.MessageProcessors
{
    /// <summary>
    /// Processing step that verifies whether 'From' property of the email is valid
    /// </summary>
    public class SourceEmailFilter : IProcessingStep<EmailRequestMessage>
    {
        private readonly Random _random;

        public SourceEmailFilter(Random random)
        {
            _random = random;
        }

        public Task<IEnumerable<ProcessingResult>> Process(EmailRequestMessage item)
        {
            // Imitating some business logic with rng.
            // There is 33% chance that this message will be disqualified.
            if (_random.Next(2) == 1)
            {
                return Task.FromResult(new[] { ProcessingResult.Failed(nameof(SourceEmailFilter)) }.AsEnumerable());
            }

            return Task.FromResult( new[] { ProcessingResult.Successful(nameof(SourceEmailFilter)) }.AsEnumerable());

        }
    }
}
