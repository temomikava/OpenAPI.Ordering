using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public interface IComputationQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
