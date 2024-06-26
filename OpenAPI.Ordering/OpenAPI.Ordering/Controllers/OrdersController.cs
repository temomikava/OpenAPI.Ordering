﻿using IntegrationEvents;
using Microsoft.AspNetCore.Mvc;
using OpenAPI.Ordering.Commands;
using OpenAPI.Ordering.Data;
using OpenAPI.Ordering.Dtos;
using OpenAPI.Ordering.Enums;
using OpenAPI.Ordering.Services;
using SharedKernel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenAPI.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository ordersRepo;
        private readonly IIntegrationEventService integrationEventService;
        private readonly OrderComputationService computationService;
        private readonly IRepository<Company, int> companiesRepo;
        private readonly IRepository<ComputationResult, int> computationRepo;

        public OrdersController(IRepository<Company, int> companiesRepo, IOrderRepository ordersRepo, IIntegrationEventService integrationEventService, OrderComputationService computationService, IRepository<ComputationResult, int> computationRepo)
        {
            this.companiesRepo = companiesRepo;
            this.ordersRepo = ordersRepo;
            this.integrationEventService = integrationEventService;
            this.computationService = computationService;
            this.computationRepo = computationRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromHeader] string apikey, [FromHeader] string apiSecret, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company == null)
            {
                return Unauthorized();
            }
            var orders = await ordersRepo.GetAllAsync(x => x.CompanyId == company.Id, token);
            return Ok(orders.Select(order => new OrderDto(order.Id, order.Amount, order.Currency, order.Status, order.CompanyId, order.CreatedAt)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromHeader] string apikey, [FromHeader] string apiSecret, int id, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company != null)
            {
                var order = await ordersRepo.GetByIdAsync(id);
                if (order is null)
                {
                    return NotFound();
                }
                if (order.CompanyId != company.Id)
                {
                    return Forbid();
                }
                return Ok(new OrderDto(order.Id, order.Amount, order.Currency, order.Status, order.CompanyId, order.CreatedAt));
            }

            return Unauthorized();
        }
        [HttpPost("pay")]
        public async Task<IActionResult> PayOrderAsync([FromHeader] string apikey, [FromHeader] string apiSecret, PayOrderCommand command, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company is not null)
            {
                var order = await ordersRepo.GetByIdAsync(command.OrderId);
                if (order is null)
                {
                    return NotFound();
                }
                if (order.CompanyId != company.Id)
                {
                    return Forbid();
                }

                await integrationEventService.AddEventAsync(new OrderProcessingIntegrationEvent
                {
                    OrderId = command.OrderId,
                    Amount = order.Amount,
                    Currency = order.Currency,
                    CardNumber = command.CardNumber,
                    ExpiryDate = command.ExpiryDate,
                    CompanyName = company.Name
                });
                await integrationEventService.PublishEventsAsync(Guid.NewGuid(), token);
                order.Status = OrderStatus.Processing;
                await ordersRepo.UpdateAsync(order);
                return Ok(order.Id);
            }
            return Unauthorized();
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromHeader] string apikey, [FromHeader] string apiSecret, [FromBody] CreateOrderCommand command, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);

            if (company is not null)
            {
                var totalAmount = await ordersRepo.GetTotalAmountForCompletedOrdersAsync(company.Id, DateTime.UtcNow);

                if (totalAmount + command.Amount > 10000)
                {
                    return BadRequest("Daily order limit exceeded");
                }

                var order = new Order
                {
                    Amount = command.Amount,
                    Currency = command.Currency,
                    CompanyId = company.Id,
                    Status = OrderStatus.Draft,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await ordersRepo.CreateAsync(order);
                return Ok(order.Id);
            }
            return Unauthorized();
        }
        [HttpPost("compute")]
        public async Task<IActionResult> ComputeOrdersAsync([FromHeader] string apikey, [FromHeader] string apiSecret, CancellationToken token)
        {
            var company = await companiesRepo.SingleOrDefaultAsync(x => x.APIKey == apikey && x.APISecret == apiSecret, token);
            if (company is not null)
            {
                var taskId = await computationService.QueueOrderComputationAsync(company.Id, token);
                return Ok(new { TaskId = taskId });
            }
            return Unauthorized();
        }

        [HttpGet("status/{taskId}")]
        public async Task<IActionResult> GetTaskStatus(string taskId, CancellationToken token)
        {
            var result = await computationRepo.SingleOrDefaultAsync(x => x.TaskId == taskId, token);
            if (result != null)
            {
                if (result.Status == ComputationResultStatus.Completed)
                {
                    return Ok(new { Status = "Completed", Result = result.Result });
                }
                else
                {
                    return Ok(new { Status = "Pending" });
                }
            }
            return NotFound();
        }
    }
}
