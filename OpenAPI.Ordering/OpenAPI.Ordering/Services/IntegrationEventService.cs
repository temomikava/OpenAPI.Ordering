
using MassTransit;
using SharedKernel;

namespace OpenAPI.Ordering.Services
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly List<IIntegrationEvent> events = new List<IIntegrationEvent>();
        private readonly ILogger<IntegrationEventService> logger;

        public IntegrationEventService(IPublishEndpoint publishEndpoint, ILogger<IntegrationEventService> logger)
        {
            _publishEndpoint = publishEndpoint;
            this.logger = logger;
        }
        public Task AddEventAsync(BaseIntegrationEvent @event)
        {
            events.Add(@event);
            return Task.CompletedTask;
        }

        public async Task PublishEventsAsync(Guid correlationId, CancellationToken token)
        {
            logger.LogInformation("sending message");
            foreach (var @event in events)
            {
                await _publishEndpoint.Publish(@event, @event.GetType(), c =>
                {
                    c.CorrelationId = correlationId;
                }, token);
            }
        }
    }
}
