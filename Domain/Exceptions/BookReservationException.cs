using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class BookReservationException : Exception
    {
        public BookReservationException(string message)
            : base(message)
        {
        }
    }
}
