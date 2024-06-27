using SharedKernel;

namespace OpenAPI.Ordering
{
    public class BackgroundOrderComputationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BackgroundOrderComputationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var computationQueue = scope.ServiceProvider.GetRequiredService<IComputationQueue>();
                    var task = await computationQueue.DequeueAsync(stoppingToken);

                    if (task != null)
                    {
                        await task(stoppingToken);
                    }
                }
            }
        }
    }
}
