using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Interface for publishing domain events.
    /// </summary>
    public interface IDomainEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }
}
