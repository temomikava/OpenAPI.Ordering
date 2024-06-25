namespace OpenAPI.Ordering
{
    public class PayOrderCommand
    {
        public int OrderId { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
