using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reservations
{
    public class GetReservationDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime EndsAt { get; set; }
        public bool IsReturned { get; set; } 
    }
}
