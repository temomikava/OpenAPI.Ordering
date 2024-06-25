using IntegrationEvents;
using MassTransit;
using OpenAPI.Ordering.Data;
using SharedKernel;

namespace OpenAPI.Ordering.IntegrationEventHandlers
{
    public class OrderRejectedIntegrationEventHandler : IIntegrationEventHandler<OrderRejectedIntegrationEvent>
    {
        private readonly ILogger<OrderRejectedIntegrationEventHandler> logger;
        private readonly IRepository<Order, int> repository;

        public OrderRejectedIntegrationEventHandler(ILogger<OrderRejectedIntegrationEventHandler> logger,IRepository<Order,int> repository)
        {
            this.logger = logger;
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<OrderRejectedIntegrationEvent> context)
        {
            logger.LogInformation($"event {nameof(OrderRejectedIntegrationEvent)} consumed successfully.");
            var order = await repository.GetByIdAsync(context.Message.OrderId);
            if (order == null)
            {
                throw new Exception($"Order with id {context.Message.OrderId} not found");
            }
            order.Status = OrderStatus.Rejected;
            await repository.UpdateAsync(order);
        }
    }
}
