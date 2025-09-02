using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Authors
{
    public class UpdateAuthorDTO
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? MiddleName { get; set; }
        public int Age { get; set; }
        public List<Book> Books { get; set; }
    }
}
