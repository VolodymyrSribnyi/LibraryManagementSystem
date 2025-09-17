using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Books
{
    //    + Id: Guid
    //+ Title: string
    //+ Author: Author
    //+Genre: enum
    //+ Publisher: string
    //+ PublishingYear: int
    public class UpdateBookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public string Publisher { get; set; }
        public int PublishingYear { get; set; }
        public List<Author> Authors { get; set; } = [];
    }
}
