using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    //    + Id: Guid
    //+ Title: string
    //Author: Author
    //+Genre: enum
    //+ Publisher: string
    //+ PublishingYear: int
    //+ CreatedAt: DateTime
    //+ IsAvailable: bool
    //+ Rating : double
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public Genre Genre { get; set; }
        public string Publisher { get; set; }
        public int PublishingYear { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsAvailable { get; set; }
        public Rating Rating { get; set; }
        //public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
    }
}
