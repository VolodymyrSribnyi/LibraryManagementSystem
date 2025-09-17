using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ReservationNotFoundException : NotFoundException
    {
        public ReservationNotFoundException(string message) : base(message)
        {
        }
    }
}
