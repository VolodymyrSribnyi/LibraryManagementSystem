using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class LibraryCardNotFoundException : NotFoundException
    {
        public LibraryCardNotFoundException(Guid id) : base($"Library card with id {id} not found")
        {
        }

        public LibraryCardNotFoundException(string userName) : base($"Library card with username {userName} not found")
        {
        }
    }
}
