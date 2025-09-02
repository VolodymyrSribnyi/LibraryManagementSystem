using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Books
{
    public class UpdateBookStatusDTO
    {
        public Guid BookId { get; set; }
        public bool IsAvailable { get; set; } // true if available, false if checked out
    }
}
