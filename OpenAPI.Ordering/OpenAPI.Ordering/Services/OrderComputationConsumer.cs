using MassTransit;
using OpenAPI.Ordering.Data;
using OpenAPI.Ordering.Enums;
using SharedKernel;

namespace OpenAPI.Ordering.Services
{
    public class OrderComputationConsumer : IIntegrationEventHandler<OrderComputationEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderComputationConsumer> _logger;

        public OrderComputationConsumer(IServiceProvider serviceProvider, ILogger<OrderComputationConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderComputationEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;

            using (var scope = _serviceProvider.CreateScope())
            {
                var computationRepository = scope.ServiceProvider.GetRequiredService<IRepository<ComputationResult, int>>();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IRepository<Order, int>>();

                var orders = await orderRepository.GetAllAsync(x => x.CompanyId == message.CompanyId, cancellationToken);

                _logger.LogInformation($"Starting computation for TaskId: {message.TaskId}");

                var result = await ComputeOrdersAsync(orders, context.CancellationToken);

                var existingResult = await computationRepository.SingleOrDefaultAsync(x => x.TaskId == message.TaskId, cancellationToken);
                if (existingResult != null)
                {
                    existingResult.Result = result;
                    existingResult.Status = ComputationResultStatus.Completed;
                    await computationRepository.UpdateAsync(existingResult);

                    _logger.LogInformation($"Order computation completed for TaskId: {message.TaskId}");
                }
            }
        }

        private async Task<decimal?> ComputeOrdersAsync(List<Order> orders, CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromMinutes(2), token);
            return orders?.Sum(x => x.Amount);
        }
    }

}
