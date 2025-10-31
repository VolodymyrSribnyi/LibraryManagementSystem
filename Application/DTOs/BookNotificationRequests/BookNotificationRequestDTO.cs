using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BookNotificationRequests
{
    public class BookNotificationRequestDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? BookId { get; set; }
    }
}
