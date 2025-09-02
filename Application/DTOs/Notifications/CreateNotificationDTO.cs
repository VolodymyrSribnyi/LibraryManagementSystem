using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Notitfications
{
    public class CreateNotificationDTO
    {
        public Guid UserId { get; set; }
        public Guid? BookId { get; set; }
        public string Message { get; set; }
        public NotificationType NotificationType { get; set; }

    }
}
