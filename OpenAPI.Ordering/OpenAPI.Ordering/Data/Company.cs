namespace OpenAPI.Ordering.Data
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
        public List<Order> Orders { get; set; }
    }
}
