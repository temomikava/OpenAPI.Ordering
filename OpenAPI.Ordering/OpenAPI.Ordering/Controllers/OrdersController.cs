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
        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<List<Order>> Get(CancellationToken token)
        {
            return await ordersRepo.GetAllAsync(token);
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] string apikey, [FromHeader] string apiSecret, [FromBody] CreateOrderCommand command, CancellationToken token)
        {
            var isAuthenticated = await companiesRepo.AnyAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (isAuthenticated)
            {
                var order = new Order
                {
                    Amount = command.Amount,
                    Currency = command.Currency
                };
                await ordersRepo.CreateAsync(order);
                return Ok(order.Id);
            }
            return Unauthorized();
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
