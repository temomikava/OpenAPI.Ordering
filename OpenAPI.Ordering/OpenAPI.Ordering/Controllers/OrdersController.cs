using Microsoft.AspNetCore.Mvc;
using OpenAPI.Ordering.Data;
using SharedKernel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenAPI.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order, int> ordersRepo;

        private readonly IRepository<Company, int> companiesRepo;

        public OrdersController(IRepository<Company, int> companiesRepo,IRepository<Order,int> ordersRepo)
        {
            this.companiesRepo = companiesRepo;
            this.ordersRepo = ordersRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string apikey, [FromHeader] string apiSecret, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company == null)
            {
                return Unauthorized();
            }
            var orders = await ordersRepo.GetAllAsync(x => x.CompanyId == company.Id, token);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromHeader] string apikey, [FromHeader] string apiSecret, int OrderId, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company != null)
            {
                var order = await ordersRepo.GetByIdAsync(OrderId);
                if (order is null)
                {
                    return NotFound();
                }
                if (order.CompanyId != company.Id)
                {
                    return Forbid();
                }
                return Ok(order);
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string apikey, [FromHeader] string apiSecret, [FromBody] CreateOrderCommand command, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company is not null)
            {
                var order = new Order
                {
                    Amount = command.Amount,
                    Currency = command.Currency,
                    CompanyId = company.Id,
                    Status = OrderStatus.Draft                   
                };
                await ordersRepo.CreateAsync(order);
                return Ok(order.Id);
            }
            return Unauthorized();
        }
    }
}
