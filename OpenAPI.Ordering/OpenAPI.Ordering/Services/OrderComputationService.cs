using OpenAPI.Identity.Data;
using OpenAPI.Ordering.Data;
using OpenAPI.Ordering.Enums;
using SharedKernel;
using System;

namespace OpenAPI.Ordering.Services
{
    public class OrderComputationService
    {
        private readonly IRepository<ComputationResult, int> _computationRepository;
        private readonly IIntegrationEventService _integrationEventService;

        public OrderComputationService(IRepository<ComputationResult, int> computationRepository, IIntegrationEventService integrationEventService)
        {
            _computationRepository = computationRepository;
            _integrationEventService = integrationEventService;
        }

        public async Task<string> QueueOrderComputationAsync(int companyId, CancellationToken token)
        {
            var taskId = Guid.NewGuid().ToString();
            var computationResult = new ComputationResult
            {
                TaskId = taskId,
                Status = ComputationResultStatus.Pending,
            };
            await _computationRepository.CreateAsync(computationResult);

            var orderComputationEvent = new OrderComputationEvent
            {
                TaskId = taskId,
                CompanyId = companyId
            };
            await _integrationEventService.AddEventAsync(orderComputationEvent);
            await _integrationEventService.PublishEventsAsync(Guid.NewGuid(), token);

            return taskId;
        }
    }
}
