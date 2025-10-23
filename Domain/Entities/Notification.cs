using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a notification sent to a user, containing information about an event or action.
    /// </summary>
    /// <remarks>A notification is associated with a specific user and may optionally reference a related
    /// book. Notifications can be marked as read or unread and include a message describing the event.</remarks>
    public class Notification
    {
        public Guid Id { get; set; }
        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public Guid? BookId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public NotificationType NotificationType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
