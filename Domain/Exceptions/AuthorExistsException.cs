using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class AuthorExistsException : BadRequestException
    {
        public AuthorExistsException(string firstName,string surname)
            : base($"An author with the name '{firstName}' and the surname '{surname}' already exists.")
        {
        }
    }
}
