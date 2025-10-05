using Application.Services.Interfaces;
using Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    /// <summary>
    /// A class for publishing domain events
    /// </summary>
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainEventPublisher> _logger;
        public DomainEventPublisher(IServiceProvider serviceProvider, ILogger<DomainEventPublisher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            // Get the handler type for the specific domain event
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            // Resolve all registered handlers for the event type
            var handlers = _serviceProvider.GetServices(handlerType);
            // Invoke each handler's HandleAsync method
            var tasks = handlers.Select(handler =>
                (Task)handlerType.GetMethod("HandleAsync").Invoke(handler, new object[]{ domainEvent }));
            
            await Task.WhenAll(tasks);

            _logger.LogInformation($"Published event {domainEvent.GetType().Name} to {handlers.Count()} handlers.");
        }
    }
}
