using SharedKernel;

namespace OpenAPI.Ordering.Data
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        Task<decimal> GetTotalAmountForCompletedOrdersAsync(int companyId, DateTime date);
    }
}
