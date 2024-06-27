using OpenAPI.Ordering.Data;
using SharedKernel;

namespace OpenAPI.Ordering
{
    public interface IOrderRepository : IRepository<Order,int>
    {
        Task<decimal> GetTotalAmountForCompletedOrdersAsync(int companyId, DateTime date);
    }
}
