using SharedKernel;

namespace IntegrationEvents
{
    public class OrderCreatedIntegrationEvent : BaseIntegrationEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency {  get; set; }
    }
}
