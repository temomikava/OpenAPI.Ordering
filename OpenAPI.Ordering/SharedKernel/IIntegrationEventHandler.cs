using MassTransit;
namespace SharedKernel
{
    public interface IIntegrationEventHandler<T> : IConsumer<T> where T : class , IIntegrationEvent
    {

    }
}
