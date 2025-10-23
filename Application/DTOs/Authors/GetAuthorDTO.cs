using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Authors
{
    public class GetAuthorDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string? MiddleName { get; set; }
        public string FullName
        {
            get
            {
                if (MiddleName == null)
                    return FirstName + " " + Surname;
                else
                    return FirstName + " " + MiddleName + " " + Surname;
            }
        }
        public int Age { get; set; }
        public string Description { get; set; }
        public List<Book> Books { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
