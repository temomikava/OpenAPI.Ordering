using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEvents
{
    public class OrderCompletedIntegrationEvent : BaseIntegrationEvent
    {
        public OrderCompletedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
        public int OrderId { get; set; }
    }
}
