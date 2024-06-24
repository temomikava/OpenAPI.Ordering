using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public interface IIntegrationEventHandler<T> : IConsumer<T> where T : class , IIntegrationEvent
    {

    }
}
