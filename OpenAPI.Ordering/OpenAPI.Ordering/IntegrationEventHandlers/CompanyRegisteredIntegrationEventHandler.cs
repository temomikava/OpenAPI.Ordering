using IntegrationEvents;
using MassTransit;
using OpenAPI.Ordering.Data;
using SharedKernel;

namespace OpenAPI.Ordering.IntegrationEventHandlers
{
    public class CompanyRegisteredIntegrationEventHandler : IIntegrationEventHandler<CompanyRegisteredIntegrationEvent>
    {
        private readonly ILogger<CompanyRegisteredIntegrationEventHandler> logger;
        private readonly IRepository<Company, int> companyRepo;

        public CompanyRegisteredIntegrationEventHandler(ILogger<CompanyRegisteredIntegrationEventHandler> logger, IRepository<Company,int> companyRepo)
        {
            this.logger = logger;
            this.companyRepo = companyRepo;
        }
        public async Task Consume(ConsumeContext<CompanyRegisteredIntegrationEvent> context)
        {
            var message = context.Message;
            var company = new Company
            {
                APIKey = message.ApiKey,
                APISecret = message.ApiSecret,
                Name = message.Name
            };
            await companyRepo.CreateAsync(company);
            logger.LogInformation($"message consumed successfully : {message.Name}");
        }
    }
}
