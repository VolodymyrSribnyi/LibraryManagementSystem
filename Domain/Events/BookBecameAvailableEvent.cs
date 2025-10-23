using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    /// <summary>
    /// Represents an event that is triggered when a book becomes available.
    /// </summary>
    /// <remarks>This event contains information about the book that became available, including its unique
    /// identifier, title, and the time the event occurred.
    /// </remarks>
    public class BookBecameAvailableEvent : IDomainEvent
    {
        public BookBecameAvailableEvent(Guid bookId,string bookTitle)
        {
            BookId = bookId;
            Title = bookTitle;
            Id = Guid.NewGuid();
            OccurredOn = DateTime.Now;
        }
        public Guid Id { get; }
        public Guid BookId { get; }
        public string Title { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
