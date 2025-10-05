using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    //    + Id: Guid
    //+ UserId: Guid
    //+ BookId: Guid
    //+ Message: string
    //+ IsRead: bool
    //+ NotificationType: enum
    //+ CreatedAt: DateTime
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
