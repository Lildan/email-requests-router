using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailRequestsRouter.Processing
{
    public interface IProcessingStep<T>
    {
        public Task<IEnumerable<ProcessingResult>> Process(T item);
    }
}
