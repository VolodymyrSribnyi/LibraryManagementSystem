using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Books
{
    public class GetBookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public string Publisher { get; set; }
        public int PublishingYear { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
        public Rating Rating { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
