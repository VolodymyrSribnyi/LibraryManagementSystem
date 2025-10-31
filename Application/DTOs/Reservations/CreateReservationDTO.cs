using Application.DTOs.Books;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reservations
{
    public class CreateReservationDTO
    {
        public Guid BookId { get; set; }
        public GetBookDTO Book { get; set; }
        public Guid UserId { get; set; }
    }
}
