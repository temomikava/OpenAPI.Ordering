using IntegrationEvents;
using MassTransit;
using OpenAPI.Ordering.Data;
using OpenAPI.Ordering.Enums;
using SharedKernel;

namespace OpenAPI.Ordering.IntegrationEventHandlers
{
    public class OrderCompletedIntegrationEventHandler : IIntegrationEventHandler<OrderCompletedIntegrationEvent>
    {
        private readonly ILogger<OrderRejectedIntegrationEventHandler> logger;
        private readonly IRepository<Order, int> repository;

        public OrderCompletedIntegrationEventHandler(ILogger<OrderRejectedIntegrationEventHandler> logger, IRepository<Order, int> repository)
        {
            this.logger = logger;
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<OrderCompletedIntegrationEvent> context)
        {
            logger.LogInformation($"event {nameof(OrderCompletedIntegrationEvent)} consumed successfully.");
            var order = await repository.GetByIdAsync(context.Message.OrderId);
            if (order == null)
            {
                throw new Exception($"Order with id {context.Message.OrderId} not found");
            }
            order.Status = OrderStatus.Completed;
            await repository.UpdateAsync(order);
        }
    }
}
