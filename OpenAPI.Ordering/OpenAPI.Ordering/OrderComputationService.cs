using OpenAPI.Identity.Data;
using OpenAPI.Ordering.Data;
using SharedKernel;
using System;

namespace OpenAPI.Ordering
{
    public class OrderComputationService
    {
        private readonly IComputationQueue _computationQueue;
        private readonly IRepository<ComputationResult,int> _computationRepository;

        public OrderComputationService(IComputationQueue computationQueue, IRepository<ComputationResult,int> computationRepo)
        {
            _computationQueue = computationQueue;
            _computationRepository = computationRepo;
        }

        public async Task<string> QueueOrderComputation(List<Order> orders, CancellationToken token)
        {
            var taskId = Guid.NewGuid().ToString();
            var computationResult = new ComputationResult
            {
                TaskId = taskId,
                Status = ComputationResultStatus.Pending,
            };
            await _computationRepository.CreateAsync(computationResult);

            _computationQueue.QueueBackgroundWorkItem(async token =>
            {
                var result = await ComputeOrdersAsync(orders, token);
                var existingResult = await _computationRepository.SingleOrDefaultAsync(x => x.TaskId == taskId, token);
                if (existingResult != null)
                {
                    existingResult.Result = result;
                    existingResult.Status = ComputationResultStatus.Completed;
                }
            });

            return taskId;
        }

        private async Task<decimal?> ComputeOrdersAsync(List<Order> orders, CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromMinutes(2), token); // Simulate heavy computation
            return orders?.Sum(x => x.Amount);
        }
    }
}
