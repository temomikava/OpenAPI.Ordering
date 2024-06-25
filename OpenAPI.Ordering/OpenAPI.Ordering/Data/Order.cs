namespace OpenAPI.Ordering.Data
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public OrderStatus Status { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
