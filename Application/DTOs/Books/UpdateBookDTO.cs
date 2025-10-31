using Application.DTOs.Authors;
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
    public class UpdateBookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public GetAuthorDTO Author { get; set; }
        public Genre Genre { get; set; }
        public string Publisher { get; set; }
        public int PublishingYear { get; set; }
        public string Description { get; set; }
        public IFormFile? Picture { get; set; }
        public List<GetAuthorDTO> Authors { get; set; } = [];
    }
}
