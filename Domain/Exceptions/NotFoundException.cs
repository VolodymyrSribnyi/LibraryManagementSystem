using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string userName)
            : base($"User with username {userName} not found")
        {
        }
        protected NotFoundException(Guid id)
            : base($"User with id {id} not found")
        { }
    }
}
