using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserExistsException : BadRequestException
    {
        public UserExistsException(string email)
            : base($"A user with the email '{email}' already exists.")
        {
        }
    }
}
