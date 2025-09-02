using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reservations
{
    public class UpdateReservationStatusDTO
    {
        public Guid Id { get; set; }
        public bool IsReturned { get; set; } // true if the book is returned, false if not
    }
}
