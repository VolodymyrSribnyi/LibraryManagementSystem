using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a book with details such as title, author, genre, publisher, and other metadata.
    /// </summary>
    /// <remarks>This class provides properties to store information about a book, including its author,
    /// genre,  publication details, availability, and rating. It is designed to be used in systems that manage  book
    /// collections, such as libraries or bookstores.</remarks>
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
        public byte[]? PictureSource { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
