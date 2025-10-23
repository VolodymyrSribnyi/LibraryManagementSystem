using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a request to notify a user about a specific book.
    /// </summary>
    /// <remarks>This class is used to track notification requests for books, including the user associated
    /// with the request, the book in question, and whether the user has been notified.</remarks>
    public class BookNotificationRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }
        public Book Book { get; set; }
        public Guid? BookId { get; set; }

        public bool IsNotified { get; set; } = false;
    }
}
