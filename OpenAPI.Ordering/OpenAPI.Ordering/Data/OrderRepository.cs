using Microsoft.EntityFrameworkCore;
using OpenAPI.Identity.Data;
using OpenAPI.Ordering.Enums;

namespace OpenAPI.Ordering.Data
{
    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<decimal> GetTotalAmountForCompletedOrdersAsync(int companyId, DateTime date)
        {
            return await context.Set<Order>()
                                .Where(o => o.CompanyId == companyId && o.Status == OrderStatus.Completed && o.CreatedAt.Date == date.Date)
                                .SumAsync(o => o.Amount);
        }
    }
}
