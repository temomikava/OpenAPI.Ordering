namespace OpenAPI.Ordering.Commands
{
    public class CreateOrderCommand
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
