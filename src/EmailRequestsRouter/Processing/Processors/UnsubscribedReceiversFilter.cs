using EmailRequestsRouter.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailRequestsRouter.Processing.Processors
{
    /// <summary>
    /// This is an example of processing step that verifies whether recipients are valid are removes those that are unsubscribed.
    /// </summary>
    public class UnsubscribedReceiversFilter: IProcessingStep<EmailRequestMessage>
    {
        // Simulate some business logic. Removing every other recipient
        public Task<IEnumerable<ProcessingResult>> Process(EmailRequestMessage item)
        {
            var recipientsList = item.EmailPayload.To.Split(',').ToList();
            if (recipientsList.Count == 1)
            {
                return Task.FromResult(new[] { ProcessingResult.Successful(nameof(UnsubscribedReceiversFilter), "All recipients are valid.") }.AsEnumerable());
            }

            // Removing 'invalid' recipients and mutating the message object
            var processingResults = 
                recipientsList
                .Where((item, index) => index % 2 != 0)
                .Select(x => ProcessingResult.Successful(nameof(UnsubscribedReceiversFilter), $"Removed {x} from recipients list."))
                .ToList();
            
            item.EmailPayload.To = string.Join(',', recipientsList.Where((item, index) => index % 2 == 0));

            return Task.FromResult(processingResults.AsEnumerable());
        }
    }
}
