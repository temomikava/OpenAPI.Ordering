namespace OpenAPI.Ordering.Commands
{
    public class PayOrderCommand
    {
        public int OrderId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
    }
}
