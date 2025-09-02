using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LibraryCards
{
    public class UpdateLibraryCardDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
