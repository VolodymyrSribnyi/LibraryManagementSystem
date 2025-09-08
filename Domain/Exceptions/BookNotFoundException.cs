using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(string message)
            :base(message) { }
        public BookNotFoundException(Guid id)
            : base($"Book with id {id} not found")
        {
        }
        public BookNotFoundException(IEnumerable<Genre> genres)
            : base("No books found for the specified genres")
        { }
    }
}
