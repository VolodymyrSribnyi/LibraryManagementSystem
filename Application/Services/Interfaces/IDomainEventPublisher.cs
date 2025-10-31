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
        /// <summary>
        /// Publishes the specified domain event asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">Event of type <see cref="IDomainEvent"></see></typeparam>
        /// <param name="domainEvent"></param>
        /// <returns>Task of published event</returns>
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }
}
