using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Notitfications
{
    public class UpdateStatusNotificationDTO
    {
        public Guid Id { get; set; }
        public bool IsRead { get; set; }
    }
}
