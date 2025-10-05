using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Interface for handling domain events.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);

    }
}
