using SharedKernel;

namespace IntegrationEvents
{
    public class OrderCompletedIntegrationEvent : BaseIntegrationEvent
    {
        public OrderCompletedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
        public int OrderId { get; set; }
    }
}
