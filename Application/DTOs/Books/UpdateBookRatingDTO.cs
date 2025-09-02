using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Books
{
    public class UpdateBookRatingDTO
    {
        public Guid BookId { get; set; }
        public Rating Rating { get; set; }
    }
}
