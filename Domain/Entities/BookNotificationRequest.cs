using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
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
