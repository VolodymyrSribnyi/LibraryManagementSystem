using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
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
