using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class BookExistsException : BadRequestException
    {
        public BookExistsException(string title) : base($"Book with title {title} exists")
        {
        }

        public BookExistsException(Guid id) : base($"Book with id {id} exists")
        {
        }
    }
}
