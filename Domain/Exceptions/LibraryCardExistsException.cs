using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class LibraryCardExistsException : BadRequestException
    {
        public LibraryCardExistsException(string name) : base($"User with name {name} has library card")
        {
        }

        public LibraryCardExistsException(Guid guid) : base($"User with id {guid} has library card")
        { }
    }
}
