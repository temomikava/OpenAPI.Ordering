namespace SharedKernel
{
    public interface IIntegrationEventService
    {
        Task AddEventAsync(BaseIntegrationEvent @event);
        Task PublishEventsAsync(Guid correlationId, CancellationToken token);
    }
}
