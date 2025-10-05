using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserIsSubscribedException : BadRequestException
    {
        public UserIsSubscribedException(string message) : base(message)
        {
        }
    }
}
