namespace OpenAPI.Ordering.Dtos
{
    public record OrderDto(int Id, decimal Amount, string Currency, OrderStatus Status, int CompanyId);
}
