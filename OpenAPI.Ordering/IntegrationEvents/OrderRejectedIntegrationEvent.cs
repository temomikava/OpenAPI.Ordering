using SharedKernel;

namespace IntegrationEvents
{
    public class OrderRejectedIntegrationEvent : BaseIntegrationEvent
    {
        public OrderRejectedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
        public int OrderId { get; set; }
    }
}
