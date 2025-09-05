using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class AuthorNotFoundException : NotFoundException
    {
        public AuthorNotFoundException(Guid authorId)
            : base($"The author with the identifier {authorId} was not found.")
        {
        }

        public AuthorNotFoundException(string surname)
            : base($"The author with the surname {surname} was not found.")
        {
        }
    }
}
