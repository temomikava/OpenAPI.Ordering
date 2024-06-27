using OpenAPI.Ordering.Data;
using SharedKernel;

namespace OpenAPI.Ordering
{
    public class OrderComputationEvent : BaseIntegrationEvent
    {
        public string TaskId { get; set; }
        public int CompanyId { get; set; }
    }
}
